using DAL.DAO;
using DAL.DAOFactory;
using DAL.DAOVO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using MSExcel = Microsoft.Office.Interop.Excel;
using MSWord = Microsoft.Office.Interop.Word;

namespace WebServer.Models.Report
{
    public class ReportService
    {
        public ReportService(int meetingID)
        {
            this.meetingID = meetingID;
            this.meetingInfo = getMeetingInfo();
            this.delegateInfos = getDelegateInfos();
            this.agendaInfos = getAgendaInfos();

            this.pdfFullName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~\Models\Report\Templet"), meetingID + ".PDF");
            this.tempTempletFileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~\Models\Report\Templet"), meetingID + ".docx");
        }

        private Dictionary<string, object> wherelist = new Dictionary<string, object>();
        private string templetFileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"~\Models\Report\Templet"), "templet.docx");

        private MSWord.Application app= null;
        private MSWord.Document doc = null;
        private MSWord.Bookmarks bookmarks = null;

        private MSExcel.Application eApp = null;
        private MSExcel.Workbook workbook = null;

        private object miss = Type.Missing;
        private object objTrue = true;

        private int meetingID;
        private ReportInfo.MeetingInfo meetingInfo;
        private List<ReportInfo.DelegateInfo> delegateInfos;
        private List<ReportInfo.AgendaInfo> agendaInfos;

        public string pdfFullName = "";

        private string tempTempletFileName = "";

        //正文字体大小
        private int fontSize = 12;
        //正文标题粗体程度
        private int titleBold = 2;
        //正文内容粗体程度
        private int textBold = 0;

        public string Export()
        {
            try
            {
                do
                {

                    if (File.Exists(pdfFullName))
                        File.Delete(pdfFullName);

                    //启动word
                    app = new MSWord.Application();
                    //设置应用不可见
                    app.Visible = false;

                    //创建临时模板文件
                    File.Copy(templetFileName, tempTempletFileName);
                    //打开临时模板文件

                    doc = app.Documents.Open(tempTempletFileName);

                    //写入模板
                    write();

                    //另存为pdf临时文件
                    object saveFormat = MSWord.WdSaveFormat.wdFormatPDF;
                    doc.SaveAs2(pdfFullName, ref saveFormat); 
                } while (false);

                return meetingInfo.meetingName+".pdf";
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();
                    doc = null;
                }
                if (app != null)
                {
                    app.Quit();
                    app = null;
                }
                if(File.Exists(tempTempletFileName)){
                    //删除临时模板文件
                    File.Delete(tempTempletFileName);
                }
            }
        }

        private ReportInfo.MeetingInfo getMeetingInfo()
        {
            var meetingInfo = new ReportInfo.MeetingInfo();

            var meetingDao = Factory.getInstance<MeetingDAO>();
            var meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            var meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            do
            {
                if (meetingVo == null)
                    break;

                meetingInfo.meetingName = meetingVo.meetingName;
                meetingInfo.meetingSummary = meetingVo.meetingSummary;
                meetingInfo.meetingToStartTime = meetingVo.meetingToStartTime.ToString();

                var meetingPlaceVo = meetingPlaceDao.getOne<MeetingPlaceVO>(meetingVo.meetingPlaceID);
                if (meetingPlaceVo == null)
                    break;
                meetingInfo.meetingPlaceName = meetingPlaceVo.meetingPlaceName;

            } while (false);
           
            return meetingInfo;
        }

        private List<ReportInfo.DelegateInfo> getDelegateInfos()
        {
            var delegateInfos = new List<ReportInfo.DelegateInfo>();

            var delegateDao = Factory.getInstance<DelegateDAO>();
            var personDao = Factory.getInstance<PersonDAO>();
            var deviceDao = Factory.getInstance<DeviceDAO>();

            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            var delegateVos = delegateDao.getAll<DelegateVO>(wherelist);

            do
            {
                if (delegateVos == null)
                    break;
                foreach (var vo in delegateVos)
                {
                    var delegateInfo = new ReportInfo.DelegateInfo();

                    var personVo = personDao.getOne<PersonVO>(vo.personID);
                    
                    var deviceVo = deviceDao.getOne<DeviceVO>(vo.deviceID);

                    delegateInfo.userName = personVo.personName;
                    delegateInfo.userDepartment = personVo.personDepartment;
                    delegateInfo.userJob = personVo.personJob;
                    delegateInfo.userMeetingRole = (vo.personMeetingRole == 1 ? "主持人" : (vo.personMeetingRole == 2 ? "主讲人" : "参会人员"));
                    delegateInfo.DeviceIMEI = deviceVo.IMEI;
                    delegateInfo.DeviceIndex = deviceVo.deviceIndex.ToString();

                    delegateInfos.Add(delegateInfo);
                }
            } while (false);
            return delegateInfos;
        }

        private List<ReportInfo.AgendaInfo> getAgendaInfos()
        {
            var ds = new DataSet();

            //添加议程信息
            var agendaInfos = new List<ReportInfo.AgendaInfo>();

            var agendaDao = Factory.getInstance<AgendaDAO>();
            var personDao = Factory.getInstance<PersonDAO>();
            var fileDao = Factory.getInstance<FileDAO>();

            wherelist.Clear();
            wherelist.Add("meetingID", meetingID);
            var agendaVos = agendaDao.getAll<AgendaVO>(wherelist);

            do
            {
                if (agendaVos == null)
                    break;

                agendaVos.Sort((x, y) =>
                {
                    return x.agendaIndex < y.agendaIndex ? -1 : (x.agendaIndex > y.agendaIndex ? 1 : 0);
                });
                foreach (var vo in agendaVos)
                {
                    var personVo = personDao.getOne<PersonVO>(vo.personID);

                    var agendaInfo = new ReportInfo.AgendaInfo();

                    agendaInfo.agendaID = vo.agendaID;
                    agendaInfo.agendaName = vo.agendaName;
                    agendaInfo.agendaDuration = vo.agendaDuration + "";
                    agendaInfo.agendaIndex = vo.agendaIndex + "";
                    agendaInfo.agendaSpeakerName = personVo.personName;



                    wherelist.Clear();
                    wherelist.Add("agendaID", vo.agendaID);
                    var fileVos = fileDao.getAll<FileVO>(wherelist);

                    //添加文件信息
                    var fileInfos = new List<ReportInfo.FileInfo>();
                    if (fileVos != null)
                    {
                        fileVos.Sort((x, y) =>
                        { return x.fileIndex < y.fileIndex ? -1 : (x.fileIndex > y.fileIndex ? 1 : 0); });
                        foreach (var fileVo in fileVos)
                        {                          
                            fileInfos.Add(new ReportInfo.FileInfo
                            {
                                fileName = fileVo.fileName,
                                fileSize = fileVo.fileSize + "",
                                fileID = fileVo.fileID
                            });
                        }
                    }
                    agendaInfo.fileInfos = fileInfos;

                    var voteDao = Factory.getInstance<VoteDAO>();
                    var voteOptionDao = Factory.getInstance<VoteOptionDAO>();
                    var voteOptionPersonResultDao = Factory.getInstance<VoteOptionPersonResultDAO>();

                    var voteVos = voteDao.getAll<VoteVO>(wherelist); 
                    //添加表决信息
                    var voteInfos = new List<ReportInfo.VoteInfo>();
                    if (voteVos != null)
                    {
                        voteVos.Sort((x, y) =>
                            {
                                return x.voteIndex < y.voteIndex ? -1 : (x.voteIndex > y.voteIndex ? 1 : 0);
                            });
                        foreach (var voteVo in voteVos)
                        {
                            var voteInfo = new ReportInfo.VoteInfo();
                            voteInfo.voteName = voteVo.voteName;
                            voteInfo.voteDescription = voteVo.voteDescription;
                            voteInfo.voteType = voteVo.voteType;
                            voteInfo.voteStatus = voteVo.voteStatus;

                            var optionInfos = new List<ReportInfo.OptionInfo>();

                            wherelist.Clear();
                            wherelist.Add("voteID", voteVo.voteID);
                            var voteOptionVos = voteOptionDao.getAll<VoteOptionVO>(wherelist);
                            if (voteOptionVos != null)
                            {
                                voteInfo.optionNum = voteOptionVos.Count;

                                voteOptionVos.Sort((x, y) =>
                                {
                                    return x.voteOptionIndex < y.voteOptionIndex ? -1 : (x.voteOptionIndex > y.voteOptionIndex ? 1 : 0);
                                });
                                foreach (var voteOptionVo in voteOptionVos)
                                {
                                    var optionInfo = new ReportInfo.OptionInfo();
                                    optionInfo.optionName = voteOptionVo.voteOptionName;

                                    //获取选项结果
                                    wherelist.Clear();
                                    wherelist.Add("voteOptionID", voteOptionVo.voteOptionID);
                                    var optionResultVos = voteOptionPersonResultDao.getAll<VoteOptionPersonResultVO>(wherelist);
                                    optionInfo.optionResult = optionResultVos == null ? 0 : optionResultVos.Count;

                                    optionInfos.Add(optionInfo);
                                }
                            }

                            voteInfo.optionInfos = optionInfos;

                            voteInfos.Add(voteInfo);
                        }
                    }
                    agendaInfo.voteInfos = voteInfos;

                    agendaInfos.Add(agendaInfo);
                }
            } while (false);

            return agendaInfos;
        }

        private void write()
        {
            try
            {
                //获取模板中所有书签
                bookmarks = doc.Bookmarks;

                //写入会议基本信息
                write_meetingInfo();
                //写入参会人员信息
                write_delegateInfos();
                //写入议程信息
                write_agendaInfos();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void write_meetingInfo()
        {
            MSWord.Range range = null;
            //书签
            object meetingNameBM="meetingName";
            object meetingPlaceNameBM="meetingPlaceName";
            object meetingToStartTimeBM="meetingToStartTime";
            object meetingSummaryBM="meetingSummary";
            
            //输入会议名称
            app.ActiveDocument.Bookmarks.get_Item(ref meetingNameBM).Select();
            range = app.Selection.Range;
            range.Text = meetingInfo.meetingName;

            //输入会场名称
            app.ActiveDocument.Bookmarks.get_Item(ref meetingPlaceNameBM).Select();
            range = app.Selection.Range;
            range.Text = meetingInfo.meetingPlaceName;

            //输入会议开始时间
            app.ActiveDocument.Bookmarks.get_Item(ref meetingToStartTimeBM).Select();
            range = app.Selection.Range;
            range.Text = meetingInfo.meetingToStartTime;

            //输入会议概况
            app.ActiveDocument.Bookmarks.get_Item(ref meetingSummaryBM).Select();
            range = app.Selection.Range;
            range.Text = meetingInfo.meetingSummary;
        }

        private void write_delegateInfos()
        {
            MSWord.Range range = null;
            //书签
            object delegateInfosBM = "delegateInfos";
            
            //输入参会人员信息
            app.ActiveDocument.Bookmarks.get_Item(ref delegateInfosBM).Select();
            range = app.Selection.Range;

            do
            {
                if (delegateInfos == null || delegateInfos.Count == 0)
                    break;
                //添加表格
                int NumRows = delegateInfos.Count + 1;
                int NumColumns = 5;
                MSWord.Table table = doc.Tables.Add(range, NumRows, NumColumns);
                table.Borders.Enable = 1;//默认表格无边框
                //设置表头
                string[] tableHeaders = { "姓名", "部门/单位", "职务", "会议角色", "使用设备编号" };
                for (int i = 1; i <= NumColumns; i++)
                {
                    table.Cell(1, i).Range.Text = tableHeaders[i - 1];
                    table.Cell(1, i).Range.Bold = titleBold;
                    table.Cell(1, i).Range.Font.Size = fontSize;
                }
                //设置表中内容
                for (int row = 2; row <= NumRows; row++)
                {
                    int column = 1;
                    table.Cell(row, column).Range.Text = delegateInfos[row - 2].userName;
                    column++;

                    table.Cell(row, column).Range.Text = delegateInfos[row - 2].userDepartment;
                    column++;

                    table.Cell(row, column).Range.Text = delegateInfos[row - 2].userJob;
                    column++;

                    table.Cell(row, column).Range.Text = delegateInfos[row - 2].userMeetingRole;
                    column++;

                    table.Cell(row, column).Range.Text = delegateInfos[row - 2].DeviceIndex;
                    column++;
                }
            } while (false);
        }

        private void write_agendaInfos()
        {
            //书签
            object agendaInfosBM = "agendaInfos";

            //输入议程信息
            app.ActiveDocument.Bookmarks.get_Item(ref agendaInfosBM).Select();
            do{
                if (agendaInfos == null || agendaInfos.Count == 0)
                    break;

                foreach (var agendaInfo in agendaInfos)
                {
                    app.Selection.Font.Size = fontSize;
                    app.Selection.Font.Bold = titleBold;
                    app.Selection.TypeText("议程名称: ");
                    app.Selection.Font.Bold = textBold;
                    app.Selection.TypeText(agendaInfo.agendaName);

                    app.Selection.TypeText("\t");

                    app.Selection.Font.Bold = titleBold;
                    app.Selection.TypeText("主讲人: ");
                    app.Selection.Font.Bold = textBold;
                    app.Selection.TypeText(agendaInfo.agendaSpeakerName);

                    app.Selection.TypeParagraph();//换行

                    //议程文件信息
                    if (agendaInfo.fileInfos != null && agendaInfo.fileInfos.Count != 0)
                    {
                        foreach (var fileInfo in agendaInfo.fileInfos)
                        {
                            app.Selection.TypeText("\t\t");
                            app.Selection.Font.Size = fontSize;
                            app.Selection.Font.Bold = titleBold;
                            app.Selection.TypeText("附件名称: ");
                            app.Selection.Font.Bold = textBold;
                            app.Selection.TypeText(fileInfo.fileName);

                            app.Selection.TypeText("\t");

                            app.Selection.Font.Bold = titleBold;
                            app.Selection.TypeText("大小: ");
                            app.Selection.Font.Bold = textBold;
                            app.Selection.TypeText(fileInfo.fileSize + "KB");
                            app.Selection.TypeParagraph();//换行
                        }
                    }

                    //议程表决信息
                    if (agendaInfo.voteInfos != null && agendaInfo.voteInfos.Count != 0)
                    {
                        try
                        {
                            //使用excel绘制图表
                            eApp = new MSExcel.Application();
                            eApp.Visible = false;//不可见
                            workbook = eApp.Workbooks.Add();//增加一个workbook

                            MSExcel.Worksheet worksheet = null;

                            app.Selection.TypeText("\t\t");
                            foreach (var voteInfo in agendaInfo.voteInfos)
                            {
                                app.Selection.Font.Size = fontSize;
                                app.Selection.Font.Bold = titleBold;
                                app.Selection.TypeText("投票名称: ");
                                app.Selection.Font.Bold = textBold;
                                app.Selection.TypeText(voteInfo.voteName);

                                app.Selection.TypeText("\t");

                                app.Selection.Font.Bold = titleBold;
                                app.Selection.TypeText("类型: ");
                                app.Selection.Font.Bold = textBold;
                                app.Selection.TypeText(voteInfo.optionNum + "选" + voteInfo.voteType);

                                app.Selection.TypeParagraph();
                                app.Selection.TypeText("\t\t\t\t");

                                app.Selection.Font.Bold = titleBold;
                                app.Selection.TypeText("投票内容: ");
                                app.Selection.Font.Bold = textBold;
                                app.Selection.TypeText(voteInfo.voteDescription);
                                app.Selection.TypeParagraph();

                                //选项
                                if (voteInfo.optionInfos != null && voteInfo.optionInfos.Count != 0)
                                {
                                    foreach (var optionInfo in voteInfo.optionInfos)
                                    {
                                        app.Selection.TypeText("\t\t\t\t");
                                        app.Selection.Font.Bold = titleBold;
                                        app.Selection.TypeText("选项: ");
                                        app.Selection.Font.Bold = textBold;
                                        app.Selection.TypeText(optionInfo.optionName);
                                        if (voteInfo.voteStatus == 16) //表决结束
                                        {
                                            //app.Selection.Font.Bold = titleBold;
                                            //app.Selection.TypeText("票数: ");
                                            //app.Selection.Font.Bold = textBold;
                                            //app.Selection.TypeText(optionInfo.optionResult+"");
                                        }
                                        app.Selection.TypeParagraph();
                                    }
                                    if (voteInfo.voteStatus == 16)//表决结束，绘制对应图表
                                    {
                                        var data = new object[voteInfo.optionNum, 2];
                                        Enumerable.Range(0, voteInfo.optionNum).ToList().ForEach(i =>
                                        {
                                            data[i, 0] = voteInfo.optionInfos[i].optionName;
                                            data[i, 1] = voteInfo.optionInfos[i].optionResult;
                                        });

                                        worksheet = (MSExcel.Worksheet)eApp.Worksheets.Add();//增加一个worksheet
                                        //设置数据
                                        worksheet.get_Range("A2", "B" + (data.Length+1)).Value = data;
                                        worksheet.get_Range("A1").Value = "选项";
                                        worksheet.get_Range("B1").Value = "票数";

                                        MSExcel.Chart xlChart = (MSExcel.Chart)workbook.Charts.Add();
                                        xlChart.SetSourceData(worksheet.get_Range("A1", "B" + (data.Length + 1)));
                                        //设置标题
                                        xlChart.HasTitle = true;
                                        xlChart.ChartTitle.Text = voteInfo.voteName;
                                        //设置x,y轴
                                        MSExcel.Axis xAxis = (MSExcel.Axis)xlChart.Axes(MSExcel.XlAxisType.xlCategory, MSExcel.XlAxisGroup.xlPrimary);
                                        MSExcel.Axis yAxis = (MSExcel.Axis)xlChart.Axes(MSExcel.XlAxisType.xlValue, MSExcel.XlAxisGroup.xlPrimary);
                                        xAxis.HasTitle = true;
                                        xAxis.AxisTitle.Text = "选项";
                                        //设置y轴单位为1
                                        yAxis.MajorUnit = 1.0;

                                        //拷贝图表
                                        MSWord.Range wdRange = app.Selection.Range;

                                        wdRange.SetRange(wdRange.End, wdRange.End + 1);
                                        xlChart.ChartArea.Copy();
                                        wdRange.Paste();
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }
                        finally
                        {
                            if (eApp != null)
                            {
                                eApp.Quit();
                                eApp = null;
                            }
                        }
                    }
                    app.Selection.TypeParagraph();//换行
                }
            }while(false);
        }
    }
}