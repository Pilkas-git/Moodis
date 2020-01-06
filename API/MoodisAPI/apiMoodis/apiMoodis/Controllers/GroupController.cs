using apiMoodis.Models;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace apiMoodis.Controllers
{
    public class GroupController : ApiController
    {
        [HttpGet]
        [Route("api/group")]
        public IHttpActionResult GetAllGroups()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                var groups = dbContext.Groups.Select(group => new GroupFE()
                {
                    Id = group.Id,
                    DoctorId = group.DoctorId,
                    GroupName = group.GroupName
                }).ToList();
                if (groups.Count != 0)
                {
                    return Ok(groups);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet]
        [Route("api/group/byid/")]
        public IHttpActionResult GetByIdGroup(string id)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Groups.Single(groupEntity => groupEntity.Id == id);
                    var group = new GroupFE() { Id = entity.Id, DoctorId = entity.DoctorId, GroupName = entity.GroupName};
                    return Ok(group);
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpGet]
        [Route("api/group/byDoctor/")]
        public IHttpActionResult GetDoctorGroups(string doctorId)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var groups = dbContext.Groups.Where(entity => entity.DoctorId == doctorId).Select(entity => new GroupFE()
                    {
                        Id = entity.Id,
                        DoctorId = entity.DoctorId,
                        GroupName = entity.GroupName
                    }).ToList();
                    return Ok(groups);
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PostGroup([FromBody] Group group)
        {
            try
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    group.Id = Guid.NewGuid().ToString();
                    dbContext.Groups.Add(group);
                    dbContext.SaveChanges();
                    return Ok();
                }
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    var generalMessage = "Entity of type \"" + entityError.Entry.Entity.GetType().Name + "\" in state \"" + entityError.Entry.State + "\" has the following validation errors:";
                    stringBuilder.AppendLine(generalMessage);
                    foreach (var validationError in entityError.ValidationErrors)
                    {
                        var propertyErrors = "- Property: \"" + validationError.PropertyName + "\", Value: \"" + entityError.Entry.CurrentValues.GetValue<object>(validationError.PropertyName) + "\", Error: \"" + validationError.ErrorMessage + "\"";
                        stringBuilder.AppendLine(propertyErrors);
                    }
                }
                return BadRequest(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody] Group group)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Groups.Single(item => item.Id == id);
                    entity.GroupName = group.GroupName;
                    entity.DoctorId = group.DoctorId;
                    dbContext.SaveChanges();
                    return Ok();
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var entityError in ex.EntityValidationErrors)
                    {
                        var generalMessage = "Entity of type \"" + entityError.Entry.Entity.GetType().Name + "\" in state \"" + entityError.Entry.State + "\" has the following validation errors:";
                        stringBuilder.AppendLine(generalMessage);
                        foreach (var validationError in entityError.ValidationErrors)
                        {
                            var propertyErrors = "- Property: \"" + validationError.PropertyName + "\", Value: \"" + entityError.Entry.CurrentValues.GetValue<object>(validationError.PropertyName) + "\", Error: \"" + validationError.ErrorMessage + "\"";
                            stringBuilder.AppendLine(propertyErrors);
                        }
                    }
                    return BadRequest(stringBuilder.ToString());
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Groups.Single(group => group.Id == id);
                    dbContext.Groups.Remove(entity);
                    dbContext.SaveChanges();
                    return Ok();
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }
    }
}
