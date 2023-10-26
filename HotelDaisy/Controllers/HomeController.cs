using HotelDaisy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelDaisy.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			if (TempData.ContainsKey("Message"))
			{
				ViewBag.SuccessMsg = TempData["Message"].ToString();
			}
			return View();
		}
	}
}