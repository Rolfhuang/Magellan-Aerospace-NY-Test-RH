using Microsoft.AspNetCore.Mvc;
using System;
using Npgsql;
using System.Data;

namespace MagellanTest.Controllers
{
    public class ItemData
    {
        public int Id { get; set; }
        public int Cost { get; set; }
        public string ItemName { get; set; }
        public int? ParentItem { get; set; }
        public DateTime ReqDate { get; set; }
    }
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly string defaultConnection;
        public ItemsController()
        {
            defaultConnection = "Host=localhost;Port=5432;Username=postgres;Password=1215;Database=Part;";
        }
        [HttpPost("create")]
        public IActionResult CreateNewItem(ItemData newData)
        {
            try
            {
                var connection = new NpgsqlConnection(defaultConnection);
                connection.Open();
                var query = @"
                    INSERT INTO item (item_name, parent_item, cost, req_date) VALUES (@itemName, @parentItem, @cost, @reqDate) RETURNING id;
                ";
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("itemName", newData.ItemName);
                cmd.Parameters.AddWithValue("parentItem", newData.ParentItem);
                cmd.Parameters.AddWithValue("cost", newData.Cost);
                cmd.Parameters.AddWithValue("reqDate", newData.ReqDate);
                var newId = (int)cmd.ExecuteScalar();
                return Ok(newId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetItemById(int id)
        {
            try
            {
                var connection = new NpgsqlConnection(defaultConnection);
                connection.Open();
                var query = @"
                    SELECT id, item_name, parent_item, cost, req_date FROM item WHERE id = @id;
                ";
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var itemName = reader.GetString(1);
                    var parentItem = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                    var cost = reader.GetInt32(3);
                    var reqDate = reader.GetDateTime(4);

                    var item = new ItemData
                    {
                        Id = id,
                        ItemName = itemName,
                        ParentItem = parentItem,
                        Cost = cost,
                        ReqDate = reqDate,
                    };
                    return Ok(item);
                }
                else
                {
                    return NotFound($"Item with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("totalcost/{itemName}")]
        public IActionResult GetTotalCost(string itemName)
        {
            try
            {
                var connection = new NpgsqlConnection(defaultConnection);
                connection.Open();
                var query = @"
                    SELECT Get_Total_Cost(@itemName);
                ";
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("itemName", itemName);
                var totalCost = (int)cmd.ExecuteScalar();
                return Ok(totalCost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
