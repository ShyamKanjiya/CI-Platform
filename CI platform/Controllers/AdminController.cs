using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult AdminUserDetails()
        {
            IEnumerable<User> userLists = _unitOfWork.User.GetAll();
            adminUserDetails obj = new();
            obj.UserLists = userLists;
            return View(obj);
        }
    }
}
