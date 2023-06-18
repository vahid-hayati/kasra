using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LgMobileController : ControllerBase
{
    private readonly IMongoCollection<BuyPhone> _collection;

    public LgMobileController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<BuyPhone>("lg-phones");
    }

    [HttpPost("register")]
    public ActionResult<BuyPhone> Create(BuyPhone userInput)
    {
        BuyPhone phone = _collection.Find(phone => phone.PhoneModel == userInput.PhoneModel.Trim()).FirstOrDefault();

        if (phone is not null)
        {
            return BadRequest("The inventory of this phone model is complete!");
        }

        phone = new BuyPhone(
           Id: null,
           PhoneModel: userInput.PhoneModel,
           Price: userInput.Price,
           Software: userInput.Software,
           HasHeadPhones: userInput.HasHeadPhones,
           Support5G: userInput.Support5G
       );

        _collection.InsertOne(phone);

        return phone;
    }

    [HttpGet("get-by-phoneModel/{phoneModel}")]
    public ActionResult<BuyPhone> GetByPhoneModel(string phoneModel)
    {
        BuyPhone phone = _collection.Find(phone => phone.PhoneModel == phoneModel.Trim()).FirstOrDefault();

        if (phoneModel is null)
        {
            return NotFound("This model is not available!");
        }

        return phone;
    }

    [HttpGet]
    public ActionResult<List<BuyPhone>> GetAll()
    {
        List<BuyPhone> phones = _collection.Find<BuyPhone>(new BsonDocument()).ToList();

        if (!phones.Any())
        {
            return Ok("The phone list is empty.");
        }

        return phones;
    }

    [HttpPut("update/{phoneId}")]
    public ActionResult<UpdateResult> UpdatePhoneById(string phoneId, BuyPhone userIn)
    {
        var updatedPhone = Builders<BuyPhone>.Update
        .Set(doc => doc.Price, userIn.Price)
        .Set(doc => doc.HasHeadPhones, userIn.HasHeadPhones)
        .Set(doc => doc.Support5G, userIn.Support5G);

        return _collection.UpdateOne<BuyPhone>(doc => doc.Id == phoneId, updatedPhone);
    }

    [HttpDelete("delete/{phoneId}")]
    public ActionResult<DeleteResult> Delete(string phoneId)
    {
        return _collection.DeleteOne<BuyPhone>(doc => doc.Id == phoneId);
    }
}