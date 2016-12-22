using System.Collections.Generic;

using DAL.DAO;

using System;
using DAL.DAOFactory;
using DAL.DAOVO;
using DAL.DB;
using WebServer.App_Start;
using WebServer.Models.Document;

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
            IMEIs[0] = "862823023300546";
            IMEIs[1] = "862823023301916";
            IMEIs[2] = "359365002515686";
            IMEIs[3] = "862823023300520";
            
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
            personNames[0] = "张三丰";
            personNames[1] = "李四爷";
            personNames[2] = "王五哥";
            personNames[3] = "张六姐";

            for (int i = 0; i < deviceNum; i++)
            {
                persons.Add(personNames[i],
                new PersonVO
                {
                    personID = personIDs[i],
                    personName = personNames[i],
                    personDepartment = "华工" + personIDs[0],
                    personJob = "学生" + personIDs[0],
                    personDescription = "小学生",
                    personPassword = "123456",
                    personState = 0
                });
            }
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
                person_roles.Add(new Person_RoleVO { person_roleID = person_roleIDs[i], roleID = 3, personID = personIDs[i] });
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

            ///////////////////////////////////////
            Console.WriteLine("添加会议");

            int meetingID = 1;
            MeetingVO meeting = new MeetingVO();
            meeting.meetingID = meetingID;
            meeting.meetingPlaceID = meetingPlaceIDs[0];
            meeting.meetingName = "人民代表大会"+meetingID;
            meeting.meetingSummary = "自由民主";
            meeting.meetingStatus = 1;
            meeting.meetingStartedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            meeting.meetingToStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            meeting.meetingDuration = 0;
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

            //删除表决、附件、议程、参会人员
            voteOptionPersonResultDao.deleteAll_test("voteOptionPersonResult");
            voteOptionDao.deleteAll_test("voteOption");
            voteDao.deleteAll_test("vote");
            fileDao.deleteAll_test("file");
            agendaDao.deleteAll_test("agenda");
            delegateDao.deleteAll_test("delegate");

            ////////////////////////////////////////////
            Console.WriteLine("添加参会人员");
            List<DelegateVO> delegates = new List<DelegateVO>();
            int[] delegateIDs = new int[deviceNum];

            for (int i = 0; i < deviceNum; i++)
            {
                delegateIDs[i] = DelegateDAO.getID();
            }
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[0],
                    deviceID = deviceIDs[0],
                    personID = personIDs[0],
                    meetingID = meetingID,
                    personMeetingRole = 1,
                    isSignIn = false
                });
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[1],
                    deviceID = deviceIDs[1],
                    personID = personIDs[1],
                    meetingID = meetingID,
                    personMeetingRole = 0,
                    isSignIn = false
                });
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[2],
                    deviceID = deviceIDs[2],
                    personID = personIDs[2],
                    meetingID = meetingID,
                    personMeetingRole = 2,
                    isSignIn = false
                });
            delegates.Add(
                new DelegateVO
                {
                    delegateID = delegateIDs[3],
                    deviceID = deviceIDs[3],
                    personID = personIDs[3],
                    meetingID = meetingID,
                    personMeetingRole = 2,
                    isSignIn = false
                });
            foreach (DelegateVO vo in delegates)
            {
                Console.WriteLine(delegateDao.insert<DelegateVO>(vo));
            }

            //////////////////////////////////////////
            Console.WriteLine("添加议程");
            int[] agendaIDs = new int[2];
            agendaIDs[0] = 1;
            agendaIDs[1] = 2;
            //议程1
            AgendaVO agenda1 = new AgendaVO();
            agenda1.agendaID = agendaIDs[0];
            agenda1.agendaIndex = 1;
            agenda1.agendaName = "测试议程"+agendaIDs[0];
            agenda1.agendaDuration = 10;
            agenda1.meetingID = meetingID;
            agenda1.personID = personIDs[0];

            Console.WriteLine(agendaDao.insert<AgendaVO>(agenda1));
            //议程2
            agenda1.agendaID = agendaIDs[1];
            agenda1.agendaIndex = 2;
            agenda1.agendaName = "测试议程" + agendaIDs[1];
            agenda1.agendaDuration = 2;
            agenda1.meetingID = meetingID;
            agenda1.personID = personIDs[1];

            Console.WriteLine(agendaDao.insert<AgendaVO>(agenda1));


            //////////////////////////////////////////
            Console.WriteLine("添加附件");

            List<FileVO> files = new List<FileVO>();
            int tempID = 0;
            //议程1中附件1
            tempID = FileDAO.getID();
            files.Add(
                new FileVO
                {
                    agendaID = agendaIDs[0],
                    fileID = tempID,
                    fileName = "竞品dy.docx",
                    fileIndex = 1,
                    fileSize = 12,
                    filePath = @"\"+meetingID+"\\"+agendaIDs[0]+"\\竞品dy.docx"
                });
            tempID = FileDAO.getID();
            files.Add(
                 new FileVO
                 {
                     agendaID = agendaIDs[1],
                     fileID = tempID,
                     fileName = "竞品dy.docx",
                     fileIndex = 1,
                     fileSize = 13,
                     filePath = @"\"+meetingID+"\\"+agendaIDs[1]+"\\竞品dy.docx"
                 });
            tempID = FileDAO.getID();
            files.Add(
                new FileVO
                {
                    agendaID = agendaIDs[1],
                    fileID = tempID,
                    fileName = "test哈哈.docx",
                    fileIndex = 2,
                    fileSize = 14,
                    filePath = @"\" + meetingID + "\\" + agendaIDs[1] + "\\test哈哈.docx"
                });
            tempID = FileDAO.getID();
            files.Add(
               new FileVO
               {
                   agendaID = agendaIDs[1],
                   fileID = tempID,
                   fileName = "干系人登记表.xlsx",
                   fileIndex = 3,
                   fileSize = 14,
                   filePath = @"\" + meetingID + "\\" + agendaIDs[1] + "\\干系人登记表.xlsx"
               });
            tempID = FileDAO.getID();
            files.Add(
               new FileVO
               {
                   agendaID = agendaIDs[1],
                   fileID = tempID,
                   fileName = "p谷歌.pptx",
                   fileIndex = 4,
                   fileSize = 14,
                   filePath = @"\" + meetingID + "\\" + agendaIDs[1] + "\\p谷歌.pptx"
               });
            tempID = FileDAO.getID();
            files.Add(
                new FileVO
                {
                    agendaID = agendaIDs[1],
                    fileID = tempID,
                    fileName = "test.pptx",
                    fileIndex = 5,
                    fileSize = 14,
                    filePath = @"\" + meetingID + "\\" + agendaIDs[1] + "\\test.pptx"
                });
            
            DocumentService documentService = new DocumentService();
            foreach (FileVO vo in files)
            {
                Console.WriteLine(fileDao.insert<FileVO>(vo));
                //重新获取fileID，避免 索引重复引起插入出错而造成agendaID不一致
                wherelist.Clear();
                wherelist.Add("filePath", vo.filePath);
                FileVO tempVo = fileDao.getOne<FileVO>(wherelist);
                //获取源文件存储在服务器端的目录的路径，及相对路径
                string srcFileRelativeDirectory_Origin = null;
                string srcFileAbsoluteDirectory = documentService.getSrcFileAbsoluteDirectory(tempVo.agendaID, out srcFileRelativeDirectory_Origin);
                //获取源文件绝对路径名
                string srcFileAbsolutePath = srcFileAbsoluteDirectory + tempVo.fileName;
                //获取目标文件绝对路径名，和目标文件相对指定root目录的路径名
                string targetFileRelativeDirectory_Root = null;
                string targetFileAbsolutePath = documentService.getTargetFileAbsolutePath(srcFileAbsolutePath, srcFileRelativeDirectory_Origin, out targetFileRelativeDirectory_Root);
                //文件转换开始
                documentService.convertFile(srcFileAbsolutePath, targetFileAbsolutePath, targetFileRelativeDirectory_Root);
            }

            //////////////////////////////////////////


            List<VoteVO> votes = new List<VoteVO>();
            List<VoteOptionVO> vote1Options = new List<VoteOptionVO>();
            List<VoteOptionVO> vote2Options = new List<VoteOptionVO>();

            int[] voteIDs = new int[2];
            for (int i = 0; i < 2; i++)
            {
                voteIDs[i] = VoteDAO.getID();
            }

            int[] vote1OptionIDs = new int[3];
            int[] vote2OptionIDs = new int[3];
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

            #region//测试更新

            #endregion


            #region 重置数据
            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("meetingStatus", 1);
            meetingDao.update(list, meetingID);

            list.Clear();
            list.Add("meetingID", meetingID);
            List<AgendaVO> agendas = agendaDao.getAll<AgendaVO>(list);
            if (agendas != null)
            {
                foreach (AgendaVO agenda in agendas)
                {
                    list.Clear();
                    list.Add("agendaID", agenda.agendaID);
                    List<VoteVO> voteVolist = voteDao.getAll<VoteVO>(list);

                    if (voteVolist != null)
                    {
                        foreach (VoteVO vote in voteVolist)
                        {
                            //恢复表决状态
                            list.Clear();
                            list.Add("voteStatus", 1);
                            voteDao.update(list, vote.voteID);
                            //清空表决结果
                            list.Clear();
                            list.Add("voteID",vote.voteID);
                            voteOptionPersonResultDao.delete(list);

                        }
                    }
                }
            }
            #endregion

            Log.DebugInfo("测试数据初始化结束");
        }
    }
}

