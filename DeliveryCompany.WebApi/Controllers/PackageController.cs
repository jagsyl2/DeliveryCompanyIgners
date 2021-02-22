using DeliveryCompany.BusinessLayer;
using DeliveryCompany.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryCompany.WebApi.Controllers
{
    [Route("api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost]
        public void PostPackage([FromBody] Package package)
        {
            _packageService.Add(package);
        }
    }
}
