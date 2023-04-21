﻿using CommonLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObjectInfo;

namespace CSE_WebEducation.Areas.User.Controllers
{
    [Route("quan-tri/nguoi-dung")]
    public class UserController : Controller
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

            return View("~/Areas/Management/User/Views/User/Index.cshtml");
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
                SearchResponseInfo _search = ApiClient_User.Search(user.Token, user.User_Name, keysearch, p_from.ToString(), p_to.ToString(), " ");

                ViewBag.LstData = JsonConvert.DeserializeObject<List<CSE_UsersInfo>>(_search.jsondata);
                ViewBag.Paging = CommonFunc.PagingData(curentPage, p_record_on_page, (int)_search.totalrows);
                ViewBag.Record_On_Page = p_record_on_page;
                //ViewBag.UserType = user.User_Type;

                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Tìm kiếm", $"tài khoản \"{user.User_Name}\" tìm kiếm thông tin nhóm tài khoản", "Quản lý nhóm người sử dụng");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return PartialView("~/Areas/Management/User/Views/User/_Partial_List.cshtml");
        }

        [Route("chi-tiet"), HttpGet]
        [CustomActionFilter]
        public IActionResult ViewDetail(decimal user_id)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_UsersInfo user_Info = ApiClient_User.GetById(user_id, user.Token);
                ViewBag.User_Info = user_Info;

                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xem thông tin", $"tài khoản \"{user.User_Name}\" xem thông tin nhóm tài khoản. Tên nhóm NSD " + _Au_Group_Info.Group_Name, "Quản lý nhóm người sử dụng");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/User/_Partial_View.cshtml");
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
            return View("~/Areas/Management/User/Views/User/_Partial_Insert.cshtml");
        }

        [Route("them-moi"), HttpPost]
        [CustomActionFilter]
        public IActionResult Insert(CSE_UsersInfo info)
        {
            decimal _success = -1;
            string _str_error = "";
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                info.Password = CommonFunc.Encrypt_MD5(info.Password);
                info.Created_By = user.User_Name;
                info.Created_Date = DateTime.Now;
                _success = ApiClient_User.Insert(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Thêm mới tài khoản thành công!";
                }
                else if (_success == -2)
                {
                    _str_error = "Tên đăng nhập đã tồn tại trong hệ thống!";
                }
                else
                {
                    _str_error = "Thêm mới tài khoản thất bại!";
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
        public IActionResult Update(decimal user_id)
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_UsersInfo info = ApiClient_User.GetById(user_id, user.Token);
                ViewBag.User_Info = info;
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/User/_Partial_Update.cshtml");
        }

        [Route("cap-nhat"), HttpPost]
        [CustomActionFilter]
        public IActionResult Update(CSE_UsersInfo info)
        {
            decimal _success = -1;
            string _str_error = "";
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                info.Modified_By = user.User_Name;
                info.Modified_Date = DateTime.Now;

                _success = ApiClient_User.Update(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Chỉnh sửa tài khoản thành công!";
                    //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Sửa", $"tài khoản \"{user.User_Name}\" sửa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
                }
                else
                {
                    _str_error = "Chỉnh sửa tài khoản thất bại!";
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

                CSE_UsersInfo info = new CSE_UsersInfo();

                info.User_Id = user_id;
                info.Status = status;
                info.Modified_By = user.User_Name;
                info.Modified_Date = DateTime.Now;

                _success = ApiClient_User.ActiveOrUnactive(info, user.Token);

                if (_success > 0)
                {
                    _str_error = "Cập nhật trạng thái tài khoản thành công!";
                    //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Sửa", $"tài khoản \"{user.User_Name}\" sửa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
                }
                else
                {
                    _str_error = "Cập nhật trạng thái tài khoản thất bại!";
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
        public IActionResult Delete(decimal user_id)
        {
            decimal _id = -1;
            string _str_error = "Xóa tài khoản thành công!";
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                CSE_UsersInfo info = ApiClient_User.GetById(user_id, user.Token);

                CSE_UsersInfo _user_info = new CSE_UsersInfo();
                _user_info.User_Id = user_id;
                _user_info.Modified_By = user.User_Name;
                _user_info.Modified_Date = DateTime.Now;

                if (_user_info.User_Id == user.User_Id)
                {
                    _id = -1;
                    _str_error = "Không thể xóa tài khoản đang đăng nhập!";
                }
                else
                {
                    _id = ApiClient_User.Delete(_user_info, user.Token);
                    if (_id < 0)
                    {
                        _str_error = "Xóa tài khoản thất bại!";
                    }
                }
                //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Xóa", $"tài khoản \"{user.User_Name}\" xóa nhóm tài khoản. Tên nhóm NSD " + info.Group_Name, "tài khoản");
            }
            catch (Exception ex)
            {
                Logger.nlog.Error(ex.ToString());
            }
            return Json(new
            {
                success = _id,
                responseMessage = _str_error
            });
        }


        [HttpGet, Route("phan-quyen")]
        [CustomActionFilter]
        public IActionResult SetUpFunctionsInGroup(decimal userId)
        {
            var user = this.HttpContext.GetCurrentUser();
            var htmlTreeViewFunctionsInGroup = string.Empty;
            try
            {
                var info = ApiClient_User.GetById(userId, user.Token);
                ViewBag.User_Info = info;

                htmlTreeViewFunctionsInGroup = new ApiClient_User_Function().GetHtmlTreeViewFunctionsOfUser(userId, user.Token);

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
            }

            return PartialView("~/Areas/Management/User/Views/User/_Partial_Set_Functions.cshtml", htmlTreeViewFunctionsInGroup);
        }

        [HttpPost, Route("phan-quyen")]
        [CustomActionFilter]
        public IActionResult SetUpFunctionsInGroup(List<CSE_User_Function_Info> lstFunctionsOfUser, decimal userId)
        {
            var user = this.HttpContext.GetCurrentUser();
            decimal _success = -1;
            string _str_error = "Phân quyền cho tài khoản thất bại";
            try
            {
                _success = ApiClient_User_Function.ResetFunction(userId, user.Token);

                if (_success > 0)
                {
                    _str_error = "Phân quyền cho tài khoản thành công";

                    ApiClient_User_Function.UpdateFunction(lstFunctionsOfUser, user.Token);

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

        [Route("doi-mat-khau"), HttpGet]
        [CustomActionFilter(CheckRight = false)]
        public IActionResult ChangePassword()
        {
            try
            {
                var user = this.HttpContext.GetCurrentUser();
                ViewBag.User_Id = user.User_Id;
                ViewBag.User_Name = user.User_Name;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
            }
            return View("~/Areas/Management/User/Views/User/_Partial_Change_Password.cshtml");
        }

        [Route("doi-mat-khau"), HttpPost]
        [CustomActionFilter(CheckRight = false)]
        public IActionResult ChangePassword(CSE_UsersInfo user)
        {
            CSE_UsersInfo user_session = this.HttpContext.GetCurrentUser();
            //Api_TraceLog.Client_Log_Insert(this.HttpContext, "Đổi mật khẩu", $"tài khoản \"{user_session.User_Name}\" đổi mật khẩu tài khoản", "tài khoản");
            decimal _success = -1;
            string _message = "Đổi mật khẩu thất bại";
            try
            {
                CSE_UsersInfo userinfo = ApiClient_User.GetById(user.User_Id, user_session.Token);
                if (userinfo == null)
                {
                    _success = -2;
                    _message = "Đổi mật khẩu thất bại";
                    return Json(new { success = _success, message = _message });
                }

                if (userinfo.User_Id != user_session.User_Id)
                {
                    _success = -2;
                    _message = "tài khoản không có quyền thực hiện chức năng này";
                    return Json(new { success = _success, message = _message });
                }

                string encryptNewPassword = CommonFunc.Encrypt_MD5(user.Password);
                string encryptOldPassword = CommonFunc.Encrypt_MD5(user.Old_Password);

                if (encryptOldPassword != user_session.Password) //Mật khẩu cũ không khớp
                {
                    _success = -2;
                    _message = "Mật khẩu cũ không khớp";
                }
                else if (user_session.Password.Equals(encryptNewPassword)) //Mật khẩu mới = mật khẩu cũ
                {
                    _success = -3;
                    _message = "Mật khẩu mới không được trùng với mật khẩu cũ";
                }
                else
                {
                    user.Password = encryptNewPassword;
                    user.Modified_Date = DateTime.Now;
                    user.Modified_By = user_session.User_Name;
                    //user.User_Type = user_session.User_Type;

                    _success = ApiClient_User.ChangePass(user, user_session.Token);

                    if (_success > 0)
                    {

                        _message = "Đổi mật khẩu thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.ToString());
            }
            return Json(new
            {
                success = _success,
                responseMessage = _message
            });
        }
    }
}
