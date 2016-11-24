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

        /// <summary>
        /// 检查参数是否符合长度规范
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userDepartment"></param>
        /// <param name="userJob"></param>
        /// <param name="userDescription"></param>
        /// <returns></returns>
        private bool _checkFormat(string userName,string userDepartment,string userJob,string userDescription)
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

        //为参会人员提供用户列表
        public Status getAllForDelegate(out List<UserForDelegate> users)
        {
            users = new List<UserForDelegate>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("personState", 0);
            //获取未冻结的用户信息
            List<PersonVO> personVolist = personDao.getAll<PersonVO>(wherelist);

            if (personVolist.Count==0)
            {
                return Status.NONFOUND;
            }

            foreach (PersonVO vo in personVolist)
            {
                users.Add(
                    new UserForDelegate
                    {
                        userID = vo.personID,
                        userName = vo.personName
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
            try
            {
                //字符串截去头尾
                newUser.userName = newUser.userName.Trim();
                newUser.userDepartment = newUser.userDepartment.Trim();
                newUser.userJob = newUser.userJob.Trim();
                newUser.userDescription = newUser.userDescription.Trim();
                //格式是否满足需求, //人员描述可为空
                if (!_checkFormat(newUser.userName, newUser.userDepartment,
                    newUser.userJob, newUser.userDescription))
                    return Status.FORMAT_ERROR;
            }
            catch (ArgumentNullException e)
            {
                return Status.FAILURE;
            }

            PersonVO personVo = new PersonVO();
            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            personVo.personID = PersonDAO.getID();
            personVo.personName = newUser.userName;
            personVo.personDepartment = newUser.userDepartment;
            personVo.personJob = newUser.userJob;
            //默认密码："123456"
            personVo.personPassword = "123456";
            personVo.personDescription = newUser.userDescription;

            //如果插入用户失败，返回error
            if (personDao.insert<PersonVO>(personVo) != 1) 
                return Status.DATABASE_OPERATOR_ERROR;

            //如果插入用户角色关联失败，则删除之前添加的数据，并返回error
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();
            if (person_roleDao.insert<Person_RoleVO>(
                new Person_RoleVO 
                {
                    person_roleID = Person_RoleDAO.getID(),
                    personID = personVo.personID,
                    roleID = newUser.roleID
                }) != 1)
            {
                personDao.delete(personVo.personID);
                return Status.DATABASE_OPERATOR_ERROR;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 为参会人员添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Status createForDelegate(CreateUserForDelegate user)
        {
            RoleDAO roleDao = Factory.getInstance<RoleDAO>();
            List<RoleVO> roles = roleDao.getAll<RoleVO>();

            //默认是member角色
            int roleID = roles.Where(role => role.roleName == "Member").ToList()[0].roleID;

            CreateUser createUser = new CreateUser
            {
                userName = user.userName,
                userDepartment = user.userDepartment,
                userJob = user.userJob,
                userDescription = user.userDescription,
                roleID = roleID
            };
            return create(createUser);
        }

        /// <summary>
        /// 请求 所有用户的 信息
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Status getAll(out List<User> users)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            users = new List<User>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Person_RoleDAO person_roleDao = Factory.getInstance <Person_RoleDAO>();

            List<PersonVO> personVolist = personDao.getAll<PersonVO>();

            if (personVolist == null)
            {
                return Status.NONFOUND;
            }

            //如果角色列表找不到，返回error
            List<RoleForUser> roles;
            Status rolesForUser_Error = new RoleService().getAllForUser(out roles);
            if(rolesForUser_Error == Status.NONFOUND ){
                return Status.NONFOUND;
            }

            foreach(PersonVO vo in personVolist){
                //如果用户角色关联找不到，返回error
                wherelist.Clear();
                wherelist.Add("personID", vo.personID);
                List<Person_RoleVO> person_roleVolist = person_roleDao.getAll<Person_RoleVO>(wherelist);
                if (person_roleVolist == null)
                {
                    return Status.DATABASE_CONTENT_ERROR;
                }

                users.Add(new User
                {
                    userID = vo.personID,
                    userName = vo.personName,
                    userDepartment = vo.personDepartment,
                    userJob = vo.personJob,
                    //从角色列表中查询对应的角色名称
                    roleName = (roles.Where(role=>
                        role.roleID==person_roleVolist[0].roleID)
                        .Select(p=>p.roleName))
                        .ToList()[0],
                    userFreezeState = vo.personState
                });

            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，请求 指定用户的 信息
        /// </summary>
        /// <param name="person"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Status getOneUpdate(out UpdateUser user, int userID)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            user = new UpdateUser();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();

            PersonVO personVo = personDao.getOne<PersonVO>(userID);
            if (personVo == null)
            {
                return Status.NONFOUND;
            }

            //如果用户角色关联找不到，返回error
            wherelist.Clear();
            wherelist.Add("personID", personVo.personID);
            List<Person_RoleVO> person_roleVolist = person_roleDao.getAll<Person_RoleVO>(wherelist);
            if (person_roleVolist == null)
            {
                return Status.DATABASE_CONTENT_ERROR;
            }


             user.userID = personVo.personID;
             user.userName = personVo.personName;
             user.userDepartment = personVo.personDepartment;
             user.userJob = personVo.personJob;
             user.userDescription = personVo.personDescription;
             user.roleID = person_roleVolist[0].roleID; // 现在，用户与角色为 多对一 关系

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，提交 更新用户的 信息
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public Status update(UpdateUser user)
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

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();

            PersonVO personVo = personDao.getOne<PersonVO>(user.userID);
            if (personVo == null)
                return Status.NONFOUND;
            personVo.personName = user.userName;
            personVo.personDepartment = user.userDepartment;
            personVo.personJob = user.userJob;
            personVo.personDescription = user.userDescription;

            wherelist.Clear();
            wherelist.Add("personName", user.userName);
            wherelist.Add("personDepartment", user.userDepartment);
            wherelist.Add("personJob", user.userJob);
            wherelist.Add("personDescription", user.userDescription);


            //更新user表
            if( personDao.update(wherelist,user.userID) != 1 )//如果失败
            {
                return Status.DATABASE_OPERATOR_ERROR;
            }

            //更新user_role关联表
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("roleID", user.roleID);
            wherelist.Clear();
            wherelist.Add("personID", user.userID);
            if (person_roleDao.update(setlist,wherelist)!=1){//如果更新原来的关联失败
                return Status.DATABASE_OPERATOR_ERROR;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        public Status UpdateUserAvailable(int userID,int available)
        {
            //检查参数
            if (userID < 0
                || (available != 0 && available != 1))
            {
                return Status.ARGUMENT_ERROR;
            }

            Dictionary<string, object> setlist = new Dictionary<string, object>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            PersonVO personVo = personDao.getOne<PersonVO>(userID);
            if( personVo == null 
                || string.Compare(personVo.personName,"admin")==0 ) // 不允许修改超级管理员的状态)
            {
                return Status.FAILURE;
            }

            setlist.Clear();
            setlist.Add("personState",available);
            if (personDao.update(setlist,userID) != 1)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }
    }
}