using AutoMapper;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    protected APIResponse _response;

    public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
    {
        //_logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
        this._response = new APIResponse();
    }


    [HttpGet]
    public async Task<ActionResult<APIResponse>> GetVillas()
    {
        try
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();

            _response.Result = _mapper.Map<List<VillaDto>>(villaList);
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

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
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

            var Villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (Villa == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<VillaDto>(Villa);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}
        try
        {
            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            var exisitingVilla = await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower());

            if (exisitingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }
            Villa model = _mapper.Map<Villa>(createDto);


            await _dbVilla.CreateAsync(model);
            _response.Result = _mapper.Map<VillaDto>(model);
            _response.StatusCode = HttpStatusCode.Created;
            await _dbVilla.SaveAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;

    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        var apiResponse = new APIResponse();

        try
        {
            if (id == 0)
                return BadRequest(apiResponse);

            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
                return NotFound(apiResponse);

            await _dbVilla.RemoveAsync(villa);

            apiResponse.Result = _mapper.Map<VillaDto>(villa);
            apiResponse.IsSuccess = true;
        }
        catch (Exception e)
        {
            apiResponse.IsSuccess = false;
            apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return Ok(apiResponse);
    }


    //[HttpPut("id:int", Name = "UpdateVilla")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaDto updateDto)
    {
        try
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDto);

            await _dbVilla.UpdateVillaAsync(model);
            _response.StatusCode = HttpStatusCode.NoContent;
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
    [HttpPatch("id:int", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

        VillaDto villaDto = _mapper.Map<VillaDto>(villa);

        if (villa == null)
        {
            return NotFound();
        }
        patchDto.ApplyTo(villaDto, ModelState);

        Villa model = _mapper.Map<Villa>(villaDto);

        await _dbVilla.UpdateVillaAsync(model);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();
    }
}