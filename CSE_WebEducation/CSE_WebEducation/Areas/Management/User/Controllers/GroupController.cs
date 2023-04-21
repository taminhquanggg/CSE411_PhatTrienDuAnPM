﻿using CommonLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObjectInfo;

namespace CSE_WebEducation.Areas.Management.User.Controllers
{
    [Route("quan-tri/nhom-nguoi-dung")]
    public class GroupController : Controller
    {
        [Route("danh-sach"), HttpGet]
        [CustomActionFilter]
        public IActionResult Index()
        {
            try
            {
                ViewBag.Record_On_Page = CommonData.RecordsPerPage;
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }

            return View("~/Areas/Management/User/Views/Group/Index.cshtml");
        }

        [Route("tim-kiem"), HttpPost]
        [CustomActionFilter]
        public IActionResult Search(string keysearch, int curentPage, int p_record_on_page)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                p_record_on_page = p_record_on_page != 0 ? p_record_on_page : CommonData.RecordsPerPage;
                int p_to = 0;
                int p_from = CommonFunc.GetFromToPaging(curentPage, p_record_on_page, out p_to);
                SearchResponseInfo _search = ApiClient_Group.Search(user.Token, user.User_Name, keysearch, p_from.ToString(), p_to.ToString(), " ");

                ViewBag.LstData = JsonConvert.DeserializeObject<List<CSE_GroupsInfo>>(_search.jsondata);
                ViewBag.Paging = CommonFunc.PagingData(curentPage, p_record_on_page, (int)_search.totalrows);
                ViewBag.Record_On_Page = p_record_on_page;
                //ViewBag.UserType = user.User_Type;

                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Tìm kiếm", $"tài khoản \"{user.User_Name}\" tìm kiếm thông tin nhóm tài khoản", "Quản lý nhóm người sử dụng");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return PartialView("~/Areas/Management/User/Views/Group/_Partial_List.cshtml");
        }

        [Route("chi-tiet"), HttpGet]
        [CustomActionFilter(FunctionCode = "GROUP_VIEW")]
        public IActionResult ViewDetail(decimal group_id)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_GroupsInfo Group_Info = ApiClient_Group.GetById(group_id, user.Token);
                ViewBag.Group_Info = Group_Info;

                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xem thông tin", $"tài khoản \"{user.User_Name}\" xem thông tin nhóm tài khoản. Tên nhóm NSD " + _Au_Group_Info.Group_Name, "Quản lý nhóm người sử dụng");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/Group/_Partial_View.cshtml");
        }

        [Route("them-moi"), HttpGet]
        [CustomActionFilter]
        public IActionResult Insert()
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                //ViewBag.User_Type = user.User_Type;
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/Group/_Partial_Insert.cshtml");
        }

        [Route("them-moi"), HttpPost]
        [CustomActionFilter]
        public IActionResult Insert(CSE_GroupsInfo info)
        {
            decimal _success = -1;
            string _str_error = "";
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                info.Created_By = user.User_Name;
                info.Created_Date = DateTime.Now;
                _success = ApiClient_Group.Insert(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Thêm mới nhóm tài khoản thành công!";
                }
                else if(_success == -2)
                {
                    _str_error = "Tên nhóm tài khoản đã tồn tại trong hệ thống!";
                }    
                else
                {
                    _str_error = "Thêm mới nhóm tài khoản thất bại!";
                }
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return Json(new
            {
                success = _success,
                responseMessage = _str_error
            });
        }

        [Route("cap-nhat"), HttpGet]
        [CustomActionFilter]
        public IActionResult Update(decimal group_id)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_GroupsInfo info = ApiClient_Group.GetById(group_id, user.Token);
                ViewBag.Group_Info = info;
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/Group/_Partial_Update.cshtml");
        }

        [Route("cap-nhat"), HttpPost]
        [CustomActionFilter]
        public IActionResult Update(CSE_GroupsInfo info)
        {
            decimal _success = -1;
            string _str_error = "Chỉnh sửa nhóm tài khoản thành công!";
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                info.Modified_By = user.User_Name;
                info.Modified_Date = DateTime.Now;

                _success = ApiClient_Group.Update(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Chỉnh sửa nhóm tài khoản thành công!";
                    //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Sửa", $"tài khoản \"{user.User_Name}\" sửa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
                }
                else if (_success == -2)
                {
                    _str_error = "Tên nhóm tài khoản đã tồn tại trong hệ thống!";
                }
                else
                {
                    _str_error = "Chỉnh sửa nhóm tài khoản thất bại!";
                }
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return Json(new
            {
                success = _success,
                responseMessage = _str_error
            });
        }

        [Route("cap-nhat-trang-thai"), HttpPost]
        [CustomActionFilter]
        public IActionResult Update(decimal user_id, string status)
        {
            decimal _success = -1;
            string _str_error = "";
            try
            {
                var user = this.HttpContext.GetCurrentUser();

                CSE_GroupsInfo info = new CSE_GroupsInfo();

                info.Group_Id = user_id;
                info.Status = status;
                info.Modified_By = user.User_Name;
                info.Modified_Date = DateTime.Now;

                _success = ApiClient_Group.ActiveOrUnactive(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Cập nhật trạng thái nhóm tài khoản thành công!";
                    //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Sửa", $"tài khoản \"{user.User_Name}\" sửa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
                }
                else
                {
                    _str_error = "Cập nhật trạng thái nhóm tài khoản thất bại!";
                }
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return Json(new
            {
                success = _success,
                responseMessage = _str_error
            });
        }

        [Route("xoa"), HttpPost]
        [CustomActionFilter]
        public IActionResult Delete(decimal group_id)
        {
            decimal _id = -1;
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_GroupsInfo info = ApiClient_Group.GetById(group_id, user.Token);

                CSE_GroupsInfo _Au_Group_Info = new CSE_GroupsInfo();
                _Au_Group_Info.Group_Id = group_id;
                _Au_Group_Info.Modified_By = user.User_Name;
                _Au_Group_Info.Modified_Date = DateTime.Now;

                _id = ApiClient_Group.Delete(_Au_Group_Info, user.Token);

                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xóa", $"tài khoản \"{user.User_Name}\" xóa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return Json(new { success = _id });
        }

        [Route("xep-nhom-nguoi-dung/{groupId}"), HttpGet]
        [CustomActionFilter(FunctionCode = "GROUP_USER")]
        public IActionResult LoadGroupUser(decimal groupId)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_GroupsInfo group = ApiClient_Group.GetById(groupId, user.Token);
                List<CSE_UsersInfo> _lstAllUser = ApiClient_Group_User.GetByGroupId(groupId);

                List<CSE_UsersInfo> _lstUserInGoup = _lstAllUser.Where(x => x.Of_Group == 1).ToList();
                List<CSE_UsersInfo> _lstUserNotIn = _lstAllUser.Where(x => x.Of_Group == 0).ToList();

                ViewBag.ListUsersInGroup = _lstUserInGoup;
                ViewBag.ListUserNotIn = _lstUserNotIn;

                ViewBag.Group_Info = group;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
            }

            return PartialView("~/Areas/Management/User/Views/Group/Group_User_Display.cshtml");
        }

        [Route("xep-nhom-nguoi-dung"), HttpPost]
        [CustomActionFilter]
        public IActionResult SaveUserGroup(int act, decimal groupId, string userId)
        {
            decimal success = -1;
            string _str_error = "Gán/ gỡ tài khoản thất bại";
            try
            {
                CSE_UsersInfo user = this.HttpContext.GetCurrentUser();
                string[] _lst_user_id = userId.Split(',');
                foreach (string item in _lst_user_id)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        CSE_Group_User_Info groupUser = new CSE_Group_User_Info()
                        {
                            User_Id = Convert.ToDecimal(item),
                            Group_Id = groupId,
                            Created_By = user?.User_Name,
                            Created_Date = DateTime.Now
                        };

                        //CSE_UsersInfo userinfo = ApiClient_User.User_GetDetail_By_Id(Convert.ToDecimal(item), user.Token);
                        //CSE_GroupsInfo groupInfo = ApiClient_Group.GetById(groupId, user.Token);

                        if (act == 1)
                        {
                            _str_error = "Gán tài khoản vào nhóm thành công!";
                            success = ApiClient_Group_User.AddUserToGroup(groupUser, user.Token);
                            //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xếp nhóm NSD", $"tài khoản \"{user.User_Name}\" thêm tài khoản \"{userinfo.User_Name}\" vào nhóm \"{groupInfo.Group_Name}\" ", "Nhóm tài khoản");
                        }
                        else
                        {
                            _str_error = "Gỡ tài khoản khỏi nhóm thành công!";
                            success = ApiClient_Group_User.RemoveUserFromGroup(groupUser, user.Token);
                            //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xếp nhóm NSD", $"tài khoản \"{user.User_Name}\" xóa tài khoản \"{userinfo.User_Name}\" khỏi nhóm \"{groupInfo.Group_Name}\" ", "Nhóm tài khoản");
                        }
                        if (success < 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _str_error = "Gán/ gỡ tài khoản thất bại";
                Logger.log.Error(ex.ToString());
            }
            return Json(new
            {
                success = success,
                responseMessage = _str_error
            });
        }


        [HttpGet, Route("phan-quyen")]
        [CustomActionFilter]
        public IActionResult SetUpFunctionsInGroup(decimal groupId)
        {
            var user = this.HttpContext.GetCurrentUser();
            var htmlTreeViewFunctionsInGroup = string.Empty;
            try
            {
                var groupInfo = ApiClient_Group.GetById(groupId, user.Token);
                ViewBag.Group_Info = groupInfo;

                htmlTreeViewFunctionsInGroup = new ApiClient_Group_Function().GetHtmlTreeViewFunctionsInGroup(groupId,  user.Token);

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
            }

            return PartialView("~/Areas/Management/User/Views/Group/_Partial_Set_Functions.cshtml", htmlTreeViewFunctionsInGroup);
        }

        [HttpPost, Route("phan-quyen")]
        [CustomActionFilter]
        public IActionResult SetUpFunctionsInGroup(List<CSE_Group_Function_Info> lstFunctionsInGroup, decimal groupId)
        {
            var user = this.HttpContext.GetCurrentUser();
            decimal _success = -1;
            string _str_error = "Phân quyền cho nhóm tài khoản thất bại";
            try
            {
                if (lstFunctionsInGroup?.Count > 0)
                {
                    foreach (var item in lstFunctionsInGroup)
                    {
                        item.Group_Id = groupId;
                        item.Created_By = user.User_Name;
                        item.Created_Date = DateTime.Now;
                    }
                }
                _success = new ApiClient_Group_Function().SetFunctionsInGroup(lstFunctionsInGroup, groupId, user.Token);

                if (_success > 0)
                {
                    _str_error = "Phân quyền cho nhóm tài khoản thành công";

                    ApiClient_Group_Function.SetFunctionsForUser(groupId, user.Token);
                    
                    //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Phân quyền", $"tài khoản \"{user.User_Name}\" phân quyền nhóm \"{groupInfo.Group_Name}\"", "Nhóm tài khoản");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
                _success = -1;
            }

            return Json(new
            {
                success = _success,
                responseMessage = _str_error
            });
        }
    }
}
