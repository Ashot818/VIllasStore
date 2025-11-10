using AutoMapper;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaNumberController : ControllerBase
{
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    protected APIResponse _response;

    public VillaNumberController(IVillaNumberRepository dbVillaNumber, IMapper mapper,
        IVillaRepository dbVilla
    )
    {
        _dbVillaNumber = dbVillaNumber;
        _dbVilla = dbVilla;
        _mapper = mapper;
        this._response = new APIResponse();
    }

    [HttpGet]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        try
        {
            IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");

            _response.Result = _mapper.Map<IEnumerable<VillaNumberDto>>(villaNumberList);
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }


        return _response;
    }

    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                //_logger.Log("Get Villa Error with Id " + id, "error");
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;

                return BadRequest(_response);
            }

            var Villa = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);

            if (Villa == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<VillaNumberDto>(Villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;

    }

    [HttpPost]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber(VillaNumberCreateDto villaNumberDto)
    {
        try
        {
            if (villaNumberDto == null)
            {
                return BadRequest(villaNumberDto);
            }

            if (await _dbVillaNumber.GetAsync(u => u.VillaNo == villaNumberDto.VillaNo) != null)
            {
                ModelState.AddModelError("CustomError", "VillaNumber Already Exists");
                return BadRequest(ModelState);
            }

            if (await _dbVilla.GetAsync(u => u.Id == villaNumberDto.VillaId) == null)
            {
                ModelState.AddModelError("CustomError", "VillaId is Invalid");
                return BadRequest(ModelState);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);


            await _dbVillaNumber.CreateAsync(model);
            _response.Result = _mapper.Map<VillaNumberDto>(model);
            _response.StatusCode = HttpStatusCode.Created;
            await _dbVillaNumber.SaveAsync();

            return CreatedAtRoute("GetVillaNumber", new { id = model.VillaNo }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;

    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

            if (villaNumber == null)
            {
                return NotFound();
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);

            _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, VillaNumberUpdateDto updateDto)
    {
        try
        {
            if (id == 0 || id != updateDto.VillaNo)
            {
                return BadRequest();
            }
            if (await _dbVilla.GetAsync(u => u.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("CustomError", "VillaId is Invalid");
                return BadRequest(ModelState);
            }

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDto);

            await _dbVillaNumber.UpdateVillaNumberAsync(villaNumber);

            _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

}
