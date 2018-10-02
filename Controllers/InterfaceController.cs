using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HaxRaspi_Server.Dtos;
using HaxRaspi_Server.Entities;
using HaxRaspi_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HaxRaspi_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private IConfiguration _configuration;
        private IInterfaceService _interfaceService;
        private IMapper _mapper;
        private string _token;

        public InterfaceController(IInterfaceService interfaceService, IMapper mapper, IConfiguration configuration)
        {
            _interfaceService = interfaceService;
            _mapper = mapper;
            _configuration = configuration;
            _token = _configuration.GetValue<string>("Token");
        }

        // POST api/interface/create
        [HttpPost]
        public IActionResult CreateOrUpdate([FromBody] InterfaceDto ifaceDto, [FromQuery(Name = "token")] string token = "")
        {
            try
            {
                if (!token.Equals(_token))
                {
                    return Unauthorized();
                }

                var iface = _mapper.Map<Interface>(ifaceDto);
                iface = _interfaceService.CreateOrUpdate(iface);
                ifaceDto = _mapper.Map<InterfaceDto>(iface);
                return Ok(ifaceDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/interface/{name}
        [HttpGet("{name}")]
        public ActionResult<string> GetByName(string name, [FromQuery(Name = "token")] string token = "")
        {
            try
            {
                if (!token.Equals(_token))
                {
                    return Unauthorized();
                }

                var iface = _interfaceService.GetByName(name);
                var ifaceDto = _mapper.Map<InterfaceDto>(iface);
                if (ifaceDto != null)
                {
                    return Ok(ifaceDto);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/interface
        [HttpGet]
        public ActionResult<string> GetAll([FromQuery(Name = "token")] string token = "")
        {
            try
            {
                if (!token.Equals(_token))
                {
                    return Unauthorized();
                }

                var ifaces = _interfaceService.GetAllInterfaces();
                var ifaceDtos = _mapper.Map<List<Interface>, List<InterfaceDto>>(ifaces);
                if (ifaceDtos == null)
                {
                    return StatusCode(500);
                }
                return Ok(ifaceDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}