using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;
using WebServer.Models.Role;

namespace WebServer.Models.User
{
    /// <summary>
    /// 服务操作类
    /// 处理有关用户管理的请求
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// 当前数据库中用户ID最大值
        /// </summary>
        private static int UserIDMax = Factory.getUserDAOInstance().getIDMax();

        private static int getUserID()
        {
            int userID = 0;
            Object lockObject = new object();
            lock(lockObject){
                userID = ++UserIDMax;
            }
            return userID;
        }

        /// <summary>
        /// 用户名
        /// 长度规范
        /// </summary>
        private static int UserName_MinLength = 2;
        private static int UserName_MaxLength = 12;

        /// <summary>
        /// 密码
        /// 长度规范
        /// </summary>
        private static int UserPassword_MinLength = 6;
        private static int UserPassword_MaxLength = 12;

        /// <summary>
        /// 用户单位名称
        /// 长度规范
        /// </summary>
        private static int UserDepartment_MinLength = 2;
        private static int UserDepartment_MaxLength = 20;

        /// <summary>
        /// 用户职务
        /// 长度规范
        /// </summary>
        private static int UserJob_MinLenght = 2;
        private static int UserJob_MaxLenght = 20;

        /// <summary>
        /// 用户描述
        /// 最大长度规范
        /// </summary>
        private static int UserDescription_MaxLenght = 25;

        private static int autoIncreateUserID = 24;

        /// <summary>
        /// 检查参数是否符合长度规范
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userDepartment"></param>
        /// <param name="userJob"></param>
        /// <param name="userDescription"></param>
        /// <returns></returns>
        private static bool _checkFormat(string userName,string userDepartment,string userJob,string userDescription)
        {
            return (UserName_MinLength <= userName.Length
                && UserName_MaxLength >= userName.Length
                && UserDepartment_MinLength <= userDepartment.Length
                && UserDepartment_MaxLength >= userDepartment.Length
                && UserJob_MinLenght <= userJob.Length
                && UserJob_MaxLenght >= userJob.Length
                && UserDescription_MaxLenght >= userDescription.Length
                );
        }

        public static Status getAllForDelegate(out List<UserForDelegate> users)
        {
            users = new List<UserForDelegate>();

            UserDAO userDaoProxy = Factory.getUserDAOInstance();

            List<UserVO> userVos = userDaoProxy.getUserList();

            if (userVos.Count==0)
            {
                return Status.NONFOUND;
            }

            foreach (UserVO vo in userVos)
            {
                if (vo.userAvailable == 1) // 已冻结
                {
                    continue;
                }
                users.Add(
                    new UserForDelegate
                    {
                        userID = vo.userID,
                        userName = vo.userName
                    });
            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public Status create(CreateUser newUser)
        {
            
            //字符串截去头尾
            newUser.userName = newUser.userName.Trim();
            newUser.userDepartment = newUser.userDepartment.Trim();
            newUser.userJob = newUser.userJob.Trim();
            newUser.userDescription = newUser.userDescription.Trim();
            //格式是否满足需求, //人员描述可为空
            if(!_checkFormat(newUser.userName,newUser.userDepartment,
                newUser.userJob,newUser.userDescription))
                return Status.FORMAT_ERROR;

            UserVO userVo = new UserVO();
            UserDAO userDaoProxy = Factory.getUserDAOInstance();

            userVo.userID = getUserID();
            userVo.userName = newUser.userName;
            userVo.userDepartment = newUser.userDepartment;
            userVo.userJob = newUser.userJob;
            //默认密码："123456"
            userVo.userPassword = "123456";
            userVo.userDescription = newUser.userDescription;

            //如果插入用户失败，返回error
            if (!userDaoProxy.addUser(userVo)) 
                return Status.DATABASE_OPERATOR_ERROR;

            //如果插入用户角色关联失败，则删除之前添加的数据，并返回error
            User_RoleDAO user_roleDao = Factory.getUser_RoleDAOInstance();
            if (!user_roleDao.addUser_Role(new User_RoleVO { userID = userVo.userID, roleID = newUser.roleID }))
            {
                userDaoProxy.deleteUserByUserID(userVo.userID);
                return Status.DATABASE_OPERATOR_ERROR;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 请求 所有用户的 信息
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static Status getAll(out List<User> users)
        {
            users = new List<User>();

            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            User_RoleDAO user_RoleDaoProxy = Factory.getUser_RoleDAOInstance();

            List<UserVO> userVos = userDaoProxy.getUserList();

            if (userVos == null)
            {
                return Status.NONFOUND;
            }

            //如果角色列表找不到，返回error
            List<RoleForUser> roles;
            Status rolesForUser_Error =  RoleService.getAllForUser(out roles);
            if(rolesForUser_Error == Status.NONFOUND ){
                return Status.NONFOUND;
            }

            foreach(UserVO vo in userVos){
                //如果用户角色关联找不到，返回error
                List<int> roleIDs = user_RoleDaoProxy.getRoleIDListByUserID(vo.userID);
                if (roleIDs == null)
                {
                    return Status.DATABASE_CONTENT_ERROR;
                }

                users.Add(new User
                {
                    userID = vo.userID,
                    userName = vo.userName,
                    userDepartment = vo.userDepartment,
                    userJob = vo.userJob,
                    roleName = (roles.Where(role=>role.roleID==roleIDs[0]).Select(p=>p.roleName)).ToList()[0],
                    userFreezeState = vo.userAvailable
                });

            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，请求 指定用户的 信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static Status getOneUpdate(out UpdateUser user, int userID)
        {
            user = new UpdateUser();

            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            User_RoleDAO user_RoleDaoProxy = Factory.getUser_RoleDAOInstance();

            UserVO vo = userDaoProxy.getUserByUserID(userID);
            if (vo == null)
            {
                return Status.NONFOUND;
            }


            //如果用户角色关联找不到，返回error
            List<int> roleIDs = user_RoleDaoProxy.getRoleIDListByUserID(vo.userID);
            if (roleIDs == null)
            {
                return Status.DATABASE_CONTENT_ERROR;
            }


             user.userID = vo.userID;
             user.userName = vo.userName;
             user.userDepartment = vo.userDepartment;
             user.userJob = vo.userJob;
             user.userDescription = vo.userDescription;
             user.roleID = roleIDs[0];

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，提交 更新用户的 信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Status update(UpdateUser user)
        {
            //字符串截去头尾
            user.userName = user.userName.Trim();
            user.userDepartment = user.userDepartment.Trim();
            user.userJob = user.userJob.Trim();
            user.userDescription = user.userDescription.Trim();

            //检查长度规范
            if ( !_checkFormat(user.userName, user.userDepartment,
                user.userJob, user.userDescription))
            {
                return Status.FORMAT_ERROR;
            }

            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            User_RoleDAO user_roleDao = Factory.getUser_RoleDAOInstance();

            UserVO vo = userDaoProxy.getUserByUserID(user.userID);
            if (vo == null)
                return Status.NONFOUND;
            vo.userName = user.userName;
            vo.userDepartment = user.userDepartment;
            vo.userJob = user.userJob;
            vo.userDescription = user.userDescription;

            //更新user表
            if (!userDaoProxy.updateUser(vo))//如果失败
            {
                return Status.DATABASE_OPERATOR_ERROR;
            }

            //更新user_role关联表
            if (!user_roleDao.deleteUser_RoleByUserID(user.userID)){//如果删除原来的关联失败
                return Status.DATABASE_OPERATOR_ERROR;
            }
            else //删除成功
            {
                //添加新的关联
                user_roleDao.addUser_Role(new User_RoleVO { userID=user.userID,roleID=user.roleID});
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        public static Status UpdateUserAvailable(int userID,int available)
        {
            //检查参数
            if(userID < 0 
                || (available != 0 && available != 1))
            {
                return Status.ARGUMENT_ERROR;
            }

            if( userID == 1 ) // 不允许修改超级管理员的状态)
            {
                return Status.FAILURE;
            }


            //数据库操作
            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            if (!userDaoProxy.updateUserAvailable(userID, available))
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }
    }
}