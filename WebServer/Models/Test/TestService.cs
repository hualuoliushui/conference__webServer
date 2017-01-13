using System.Collections.Generic;

using DAL.DAO;

using System;
using DAL.DAOFactory;
using DAL.DAOVO;
using DAL.DB;
using WebServer.App_Start;
using WebServer.Models.Document;
using System.IO;

namespace WebServer.Models.Test
{
    public class TestService
    {
        /// <summary>
        /// 须其他程序调用，不能在本程序调用
        /// 添加测试数据
        /// </summary>
        public static void init()
        {
            Log.DebugInfo("测试数据初始化...");

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            RoleDAO roleDao = Factory.getInstance<RoleDAO>();
            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();
            PermissionDAO permissionDao = Factory.getInstance<PermissionDAO>();
            Role_PermissionDAO role_permissionDao = Factory.getInstance<Role_PermissionDAO>();

            DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();
            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();

            FileDAO fileDao = Factory.getInstance<FileDAO>();

            VoteDAO voteDao = Factory.getInstance<VoteDAO>();

             VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
            
            VoteOptionPersonResultDAO voteOptionPersonResultDao = Factory.getInstance<VoteOptionPersonResultDAO>();

            #region 会议测试数据
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            Dictionary<string, DeviceVO> devices = new Dictionary<String, DeviceVO>();
            int deviceNum = 4;
            int[] deviceIDs = new int[deviceNum];
            string[] IMEIs = new string[deviceNum];
            for (int i = 0; i < deviceNum; i++)
            {
                deviceIDs[i] = DeviceDAO.getID();
            }
            IMEIs[0] = "862823023300520";
            IMEIs[1] = "862823023301916";
            IMEIs[2] = "359365002515686";
            IMEIs[3] = "862823023300546";
            
           for(int i = 0 ; i < deviceNum;i++){
               devices.Add(IMEIs[i],
                new DeviceVO
                {
                    deviceID = deviceIDs[i],
                    IMEI = IMEIs[i],
                    deviceIndex = i+1,
                    deviceState = 0
                });
           }

            Console.WriteLine("添加设备");
           
            for (int i = 0; i < devices.Count;i++)
            {
                wherelist.Clear();
                wherelist.Add("IMEI", IMEIs[i]);
                DeviceVO tempVo = deviceDao.getOne<DeviceVO>(wherelist);
                if (tempVo != null)
                {
                    devices[IMEIs[i]] = tempVo;
                    deviceIDs[i] = tempVo.deviceID;
                    continue;
                }
                Console.WriteLine(deviceDao.insert<DeviceVO>(devices[IMEIs[i]]));
            }



            Dictionary<String, MeetingPlaceVO> meetingPlaces = new Dictionary<string, MeetingPlaceVO>();
            int meetingPlaceNum = 2;
            int[] meetingPlaceIDs = new int[meetingPlaceNum];
            string[] meetingPlaceNames = new string[meetingPlaceNum];
            for (int i = 0; i < meetingPlaceNum; i++)
            {
                meetingPlaceIDs[i] = MeetingPlaceDAO.getID();
            }
            meetingPlaceNames[0] = "学术会议室";
            meetingPlaceNames[1] = "决策室";
            for (int i = 0; i < meetingPlaceNum; i++)
            {
                meetingPlaces.Add(meetingPlaceNames[i],
                    new MeetingPlaceVO
                    {
                        meetingPlaceID = meetingPlaceIDs[i],
                        meetingPlaceName = meetingPlaceNames[i],
                        meetingPlaceCapacity = 200,
                        meetingPlaceState = 0
                    });
            }

            //////////////////////////////////
            Console.WriteLine("添加会场");
            for (int i = 0; i < meetingPlaceNum;i++)
            {
                wherelist.Clear();
                wherelist.Add("meetingPlaceName",meetingPlaceNames[i]);
                MeetingPlaceVO tempVo = meetingPlaceDao.getOne<MeetingPlaceVO>(wherelist);
                if (tempVo != null)
                {
                    meetingPlaces[meetingPlaceNames[i]] = tempVo;
                    meetingPlaceIDs[i] = tempVo.meetingPlaceID;
                    continue;
                }
                Console.WriteLine(meetingPlaceDao.insert<MeetingPlaceVO>(meetingPlaces[meetingPlaceNames[i]]));
            }


            int[] personIDs = new int[deviceNum];
            string[] personNames = new string[deviceNum];
            Dictionary<string, PersonVO> persons = new Dictionary<string, PersonVO>();
            for (int i = 0; i < deviceNum; i++)
            {
                personIDs[i] = PersonDAO.getID();
            }
            personNames[0] = "彭洪云";
            personNames[1] = "赵丽萍";
            personNames[2] = "黄文欣";
            personNames[3] = "林连南";

            int personIndex = 0;
            persons.Add(personNames[personIndex],
               new PersonVO
               {
                   personID = personIDs[personIndex],
                   personName = personNames[personIndex],
                   personDepartment = "董事",
                   personJob = "董事长",
                   personDescription = "测试",
                   personPassword = "123456",
                   personState = 0
               });
            personIndex++;
            persons.Add(personNames[personIndex],
               new PersonVO
               {
                   personID = personIDs[personIndex],
                   personName = personNames[personIndex],
                   personDepartment = "电子商务部",
                   personJob = "股东",
                   personDescription = "测试",
                   personPassword = "123456",
                   personState = 0
               });
            personIndex++;
            persons.Add(personNames[personIndex],
               new PersonVO
               {
                   personID = personIDs[personIndex],
                   personName = personNames[personIndex],
                   personDepartment = "财务部",
                   personJob = "股东",
                   personDescription = "测试",
                   personPassword = "123456",
                   personState = 0
               });
            personIndex++;
            persons.Add(personNames[personIndex],
               new PersonVO
               {
                   personID = personIDs[personIndex],
                   personName = personNames[personIndex],
                   personDepartment = "研发一部",
                   personJob = "股东",
                   personDescription = "测试",
                   personPassword = "123456",
                   personState = 0
               });
            personIndex++;

            //////////////////////////////////////
            Console.WriteLine("添加用户");
            for (int i = 0; i < deviceNum;i++)
            {
                wherelist.Clear();
                wherelist.Add("personName", personNames[i]);
                PersonVO tempVo = personDao.getOne<PersonVO>(wherelist);
                if (tempVo != null)
                {
                    persons[personNames[i]] = tempVo;
                    personIDs[i] = tempVo.personID;
                    continue;
                }
                Console.WriteLine(personDao.insert<PersonVO>(persons[personNames[i]]));
            }

            int[] person_roleIDs = new int[deviceNum];
            for (int i = 0; i < deviceNum; i++)
            {
                person_roleIDs[i] = Person_RoleDAO.getID();
            }

            List<Person_RoleVO> person_roles = new List<Person_RoleVO>();
            //默认为无权限角色："成员"角色，roleID=3
            for (int i = 0; i < deviceNum; i++)
            {
               
                if (i == 3)
                {
                    person_roles.Add(new Person_RoleVO { person_roleID = person_roleIDs[i], roleID = 2, personID = personIDs[i] });
                }
                else
                {
                    person_roles.Add(new Person_RoleVO { person_roleID = person_roleIDs[i], roleID = 3, personID = personIDs[i] });
                }
            }
            Console.WriteLine("添加用户角色关联");
            for (int i = 0; i < deviceNum; i++)
            {
                wherelist.Clear();
                wherelist.Add("personID", personIDs[i]);
                Person_RoleVO tempVo = person_roleDao.getOne<Person_RoleVO>(wherelist);
                if (tempVo != null)
                {
                    continue;
                }
                Console.WriteLine(person_roleDao.insert<Person_RoleVO>(person_roles[i]));
            }


            //删除表决、附件、议程、参会人员、会议
            voteOptionPersonResultDao.deleteAll_test("voteOptionPersonResult");
            voteOptionDao.deleteAll_test("voteOption");
            voteDao.deleteAll_test("vote");
            fileDao.deleteAll_test("file");
            agendaDao.deleteAll_test("agenda");
            delegateDao.deleteAll_test("delegate");
            meetingDao.deleteAll_test("meeting");

            ///////////////////////////////////////
            Console.WriteLine("添加会议");

            int meetingID = MeetingDAO.getID();
            MeetingVO meeting = new MeetingVO();
            meeting.meetingID = meetingID;
            meeting.meetingPlaceID = meetingPlaceIDs[0];
            meeting.meetingName = "二零一六年年度股东大会暨股东扩大会议";
            meeting.meetingSummary = "";
            meeting.meetingStatus = 1;
            meeting.meetingStartedTime = (new DateTime(DateTime.Now.AddDays(5).Ticks));
            meeting.meetingToStartTime = (new DateTime(DateTime.Now.AddDays(6).Ticks));
            meeting.meetingDuration = 150;
            meeting.personID = personIDs[0];

            do
            {
                wherelist.Clear();
                wherelist.Add("meetingName", meeting.meetingName);
                MeetingVO tempVo = meetingDao.getOne<MeetingVO>(wherelist);
                if (tempVo != null)
                {
                    meetingID = tempVo.meetingID;
                    break;
                }
                Console.WriteLine(meetingDao.insert<MeetingVO>(meeting));
            } while (false);

            ////////////////////////////////////////////
            Console.WriteLine("添加参会人员");
            List<DelegateVO> delegates = new List<DelegateVO>();
            int[] delegateIDs = new int[deviceNum];

            for (int i = 0; i < deviceNum; i++)
            {
                delegateIDs[i] = DelegateDAO.getID();
            }
            int delegateIndex = 0;
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[delegateIndex],
                    deviceID = deviceIDs[delegateIndex],
                    personID = personIDs[delegateIndex],
                    meetingID = meetingID,
                    personMeetingRole = 1,
                    isSignIn = false
                });
            delegateIndex++;
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[delegateIndex],
                    deviceID = deviceIDs[delegateIndex],
                    personID = personIDs[delegateIndex],
                    meetingID = meetingID,
                    personMeetingRole = 0,
                    isSignIn = false
                });
            delegateIndex++;
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[delegateIndex],
                    deviceID = deviceIDs[delegateIndex],
                    personID = personIDs[delegateIndex],
                    meetingID = meetingID,
                    personMeetingRole = 0,
                    isSignIn = false
                });
            delegateIndex++;
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[delegateIndex],
                    deviceID = deviceIDs[delegateIndex],
                    personID = personIDs[delegateIndex],
                    meetingID = meetingID,
                    personMeetingRole = 2,
                    isSignIn = false
                });
            delegateIndex++;
            foreach (DelegateVO vo in delegates)
            {
                Console.WriteLine(delegateDao.insert<DelegateVO>(vo));
            }

            //////////////////////////////////////////
            Console.WriteLine("添加议程");
            int agendaNum = 3;
            Dictionary<int, AgendaVO> agendas = new Dictionary<int, AgendaVO>();
            int[] agendaIDs = new int[agendaNum];
            for (int i = 0; i < agendaNum; i++)
            {
                agendaIDs[i] = AgendaDAO.getID();
            }

            int agendaIndex = 0;
            agendas.Add(agendaIDs[agendaIndex],
                new AgendaVO
                {
                    agendaID=agendaIDs[agendaIndex],
                    agendaIndex=agendaIndex+1,
                    agendaName="普通决议案",
                    agendaDuration=50,
                    meetingID = meetingID,
                    personID = personIDs[3],//主讲人
                    isUpdate = false
                });
            agendaIndex++;
            agendas.Add(agendaIDs[agendaIndex],
                new AgendaVO
                {
                    agendaID = agendaIDs[agendaIndex],
                    agendaIndex = agendaIndex + 1,
                    agendaName = "具体项目负责人汇报",
                    agendaDuration = 50,
                    meetingID = meetingID,
                    personID = personIDs[0],
                    isUpdate = false
                });
            agendaIndex++;
            agendas.Add(agendaIDs[agendaIndex],
                new AgendaVO
                {
                    agendaID = agendaIDs[agendaIndex],
                    agendaIndex = agendaIndex + 1,
                    agendaName = "特别决议案",
                    agendaDuration = 50,
                    meetingID = meetingID,
                    personID = personIDs[0],
                    isUpdate = false
                });
            agendaIndex++;
            for (int i = 0; i < agendaNum; i++)
            {
                Console.WriteLine(agendaDao.insert<AgendaVO>(agendas[agendaIDs[i]]));
            }

            //////////////////////////////////////////
            Console.WriteLine("添加附件");

            //////////////////////////////////////////


            List<VoteVO> votes = new List<VoteVO>();
            List<VoteOptionVO> vote1Options = new List<VoteOptionVO>();
            List<VoteOptionVO> vote2Options = new List<VoteOptionVO>();

            int voteNum = 2;
            int[] voteIDs = new int[voteNum];
            for (int i = 0; i < voteNum; i++)
            {
                voteIDs[i] = VoteDAO.getID();
            }

            int voteOptionNum = 3;
            int[] vote1OptionIDs = new int[voteOptionNum];
            int[] vote2OptionIDs = new int[voteOptionNum];
            for (int i = 0; i < 3; i++)
            {
                vote1OptionIDs[i] = VoteOptionDAO.getID();
                vote2OptionIDs[i] = VoteOptionDAO.getID();
            }

            Console.WriteLine("添加表决");
            //表决
            votes.Add(
                new VoteVO
                {
                    agendaID = agendaIDs[0],
                    voteID = voteIDs[0],
                    voteName = "测试表决1",
                    voteDescription = "表决谁做助理",
                    voteIndex = 1,
                    voteStatus = 1,
                    voteType = 1 //单选
                });

            votes.Add(
                new VoteVO
                {
                    agendaID = agendaIDs[1],
                    voteID = voteIDs[1],
                    voteName = "测试表决2",
                    voteDescription = "表决谁做coding",
                    voteIndex = 1,
                    voteStatus = 1,
                    voteType = 2 //最多双选
                });

            for (int i = 0; i < 3; i++)
            {
                vote1Options.Add(
                   new VoteOptionVO
                   {
                       voteID = voteIDs[0],
                       voteOptionID = vote1OptionIDs[i],
                       voteOptionIndex = 0,
                       voteOptionName = "A" + vote1OptionIDs[i]
                   });
                vote2Options.Add(
                  new VoteOptionVO
                  {
                      voteID = voteIDs[1],
                      voteOptionID = vote2OptionIDs[i],
                      voteOptionIndex = 0,
                      voteOptionName = "B" + vote2OptionIDs[i]
                  });
            }

            foreach (VoteVO vo in votes)
            {
                Console.WriteLine(voteDao.insert<VoteVO>(vo));
            }

            foreach (VoteOptionVO vo in vote1Options)
            {
                Console.WriteLine(voteOptionDao.insert<VoteOptionVO>(vo));
            }

            foreach (VoteOptionVO vo in vote2Options)
            {
                Console.WriteLine(voteOptionDao.insert<VoteOptionVO>(vo));
            }

            #endregion

            Log.DebugInfo("测试数据初始化结束");
        }
    }
}

