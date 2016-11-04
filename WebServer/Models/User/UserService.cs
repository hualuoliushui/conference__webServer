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
    public class UserService
    {
        private static int UserName_MinLength = 2;
        private static int UserName_MaxLength = 12;

        private static int UserPassword_MinLength = 6;
        private static int UserPassword_MaxLength = 12;

        private static int UserDepartment_MinLength = 2;
        private static int UserDepartment_MaxLength = 20;

        private static int UserJob_MinLenght = 2;
        private static int UserJob_MaxLenght = 20;

        private static int UserDescription_MaxLenght = 25;

        private static int autoIncreateUserID = 24;

        private static bool _trim_new(ref CreateUser newUser)
        {
            if(string.IsNullOrWhiteSpace(newUser.userName)
                || string.IsNullOrWhiteSpace(newUser.userDepartment)
                || string.IsNullOrWhiteSpace(newUser.userJob)
                || newUser.userDescription == null)
            {
                return false;
            }
            //字符串截去头尾
            newUser.userName.Trim();
            newUser.userDepartment.Trim();
            newUser.userJob.Trim();
            newUser.userDescription.Trim();

            return true;
        }

        private static bool _trim_update(ref UpdateUser updateUser)
        {
            if (string.IsNullOrWhiteSpace(updateUser.userName)
               || string.IsNullOrWhiteSpace(updateUser.userDepartment)
               || string.IsNullOrWhiteSpace(updateUser.userJob)
               || updateUser.userDescription == null)
            {
                return false;
            }
            //字符串截去头尾
            updateUser.userName.Trim();
            updateUser.userDepartment.Trim();
            updateUser.userJob.Trim();
            updateUser.userDescription.Trim();

            return true;
        }

        //检查数据，字符串长度是否符合需求
        private static bool _checkForm(string userName,string userDepartment,string userJob,string userDescription)
        {
            if(!(UserName_MinLength <= userName.Length
                && UserName_MaxLength >= userName.Length
                && UserDepartment_MinLength <= userDepartment.Length
                && UserDepartment_MaxLength >= userDepartment.Length
                && UserJob_MinLenght <= userJob.Length
                && UserJob_MaxLenght >= userJob.Length
                && UserDescription_MaxLenght >= userDescription.Length
                ))
            {
                return false;
            }
            return true;
        }

        public static Status create(CreateUser newUser)
        {
            
            //字符串截去头尾
            //格式是否满足需求, //人员描述可为空
            if (!_trim_new(ref newUser) ||
                !_checkForm(newUser.userName,newUser.userDepartment,
                newUser.userJob,newUser.userDescription))
                return Status.FORMAT_ERROR;

            UserVO userVo = new UserVO();
            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            //之后可以由数据库决定用户ID
            userVo.userID = autoIncreateUserID++;
            userVo.userName = newUser.userName;
            userVo.userDepartment = newUser.userDepartment;
            userVo.userJob = newUser.userJob;
            userVo.userPassword = "123456";
            userVo.userDescription = newUser.userDescription;

            //如果插入数据失败，返回error
            if (!userDaoProxy.addUser(userVo)) 
                return Status.DATABASE_OPERATOR_ERROR;

            //如果插入关联失败，则删除之前添加的数据，返回error
            User_RoleDAO user_roleDao = Factory.getUser_RoleDAOInstance();
            if (!user_roleDao.addUser_Role(new User_RoleVO { userID = userVo.userID, roleID = newUser.roleID }))
            {
                userDaoProxy.deleteUserByUserID(userVo.userID);
                return Status.DATABASE_OPERATOR_ERROR;
            }

            return Status.SUCCESS;
        }

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
                    roleName = (roles.Where(role=>role.roleID==roleIDs[0]).Select(p=>p.roleName)).ToList()[0]
                });

            }
            return Status.SUCCESS;
        }



        public static Status getOne(out UpdateUser user, int userID)
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

        public static Status update(UpdateUser user)
        {
            if (!_trim_update(ref user)
                ||
                !_checkForm(user.userName, user.userDepartment,
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
            if (!userDaoProxy.updateUser(vo))
            {
                return Status.DATABASE_OPERATOR_ERROR;
            }
            //更新user_role关联表

            if (!user_roleDao.deleteUser_RoleByUserID(user.userID) ||
                     !user_roleDao.addUser_Role(new User_RoleVO { userID = user.userID, roleID = user.roleID }))
                return Status.DATABASE_OPERATOR_ERROR;

            return Status.SUCCESS;
        }

        public static Status delete(List<int> users)
        {
            //通过UserID删除所有关联数据
            //周五：删除关联表
            //周六：只标志，不删除

            UserDAO userDaoProxy = Factory.getUserDAOInstance();
            User_RoleDAO user_roleDao = Factory.getUser_RoleDAOInstance();

            bool checkDelete = true;
            foreach (int id in users)
            {
                if (!user_roleDao.deleteUser_RoleByUserID(id)
                    ||
                    !userDaoProxy.deleteUserByUserID(id))
                    checkDelete = false;
            }
            if (!checkDelete)
                return Status.DATABASE_OPERATOR_ERROR;

            return Status.SUCCESS;
        }
    }
}