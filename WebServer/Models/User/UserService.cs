using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;
using WebServer.Models.Role;
using WebServer.Models.Excel;
using WebServer.App_Start;

namespace WebServer.Models.User
{
    /// <summary>
    /// 服务操作类
    /// 处理有关用户管理的请求
    /// </summary>
    public class UserService
    {
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

        //为参会人员提供用户列表//排除已有参会人员
        public Status getNewForDelegate(int meetingID, out List<UserForDelegate> users)
        {
            users = new List<UserForDelegate>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            var delegateVolist = delegateDao.getAll<DelegateVO>(wherelist);

            wherelist.Clear();
            wherelist.Add("personState", 0);
            //获取未冻结的用户信息
            List<PersonVO> personVolist = personDao.getAll<PersonVO>(wherelist);

            if (personVolist == null || personVolist.Count == 0)
            {
                return Status.NONFOUND;
            }

            foreach (PersonVO vo in personVolist)
            {
                DelegateVO delegateVoTemp = null;

                if (delegateVolist != null)
                    foreach (var delegateVo in delegateVolist)
                        if (delegateVo.personID == vo.personID)
                        {
                            delegateVoTemp = delegateVo;
                            break;
                        }

                if (delegateVoTemp == null)
                {
                    users.Add(
                    new UserForDelegate
                    {
                        userID = vo.personID,
                        userName = vo.personName
                    });
                }
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
            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();  

            PersonVO personVo = new PersonVO();

            personVo.personID = PersonDAO.getID();
            personVo.personName = newUser.userName;
            personVo.personDepartment = newUser.userDepartment;
            personVo.personJob = newUser.userJob;
            //默认密码："123456"
            personVo.personPassword = "123456";
            personVo.personDescription = newUser.userDescription;

            //如果插入用户失败，返回error
            if (personDao.insert<PersonVO>(personVo) < 0) 
                return Status.NAME_EXIST;

            //如果插入用户角色关联失败，则删除之前添加的数据，并返回error
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();
            if (person_roleDao.insert<Person_RoleVO>(
                new Person_RoleVO 
                {
                    person_roleID = Person_RoleDAO.getID(),
                    personID = personVo.personID,
                    roleID = newUser.roleID
                }) < 0)
            {
                personDao.delete(personVo.personID);
                return Status.NAME_EXIST;
            }

            return Status.SUCCESS;
        }

        public Status createMultiple(String excelFilePath,String tableName,ref List<String> checkList)
        {
            List<CreateUserForDelegate> list;
            Status status = new Excel.Excel().import<CreateUserForDelegate>(excelFilePath, tableName, out list);
            if (status != Status.SUCCESS)
            {
                return status;
            }

            foreach (CreateUserForDelegate user in list)
            {
                Status createStatus = createForDelegate(user);
                checkList.Add(Message.msgs[(int)createStatus]);
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
            //默认是“成员”角色//基础角色ID为3，默认无权限
            int roleID = 3;

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
                try
                {
                    users.Add(new User
                    {
                        userID = vo.personID,
                        userName = vo.personName,
                        userDepartment = vo.personDepartment,
                        userJob = vo.personJob,
                        //从角色列表中查询对应的角色名称
                        roleName = (roles.Where(role =>
                            role.roleID == person_roleVolist[0].roleID)
                            .Select(p => p.roleName))
                            .ToList().First(),
                        userFreezeState = vo.personState
                    });
                }
                catch (Exception e)
                {
                    Log.LogInfo("查询用户列表,填充列表",e);
                    return Status.SERVER_EXCEPTION;
                }
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
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();

            PersonVO personVo = personDao.getOne<PersonVO>(user.userID);
            if (personVo == null)
                return Status.NONFOUND;

            if (personVo.isAdmin) //不允许修改超级管理员
            {
                return Status.PERMISSION_DENIED;
            }

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
            if( personDao.update(wherelist,user.userID) < 0 )//如果失败
            {
                return Status.NAME_EXIST;
            }

            //更新user_role关联表
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("roleID", user.roleID);
            wherelist.Clear();
            wherelist.Add("personID", user.userID);
            if (person_roleDao.update(setlist,wherelist) < 0){//如果更新原来的关联失败
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
                || personVo.isAdmin ) // 不允许修改超级管理员的状态)
            {
                return Status.FAILURE;
            }

            setlist.Clear();
            setlist.Add("personState",available);
            if (personDao.update(setlist,userID) < 0)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }
    }
}