using apiMoodis.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using apiMoodis.Encryption;
using apiMoodis.Requests;
using System.Data.Entity.Validation;
using System.Text;

namespace apiMoodis.Controllers
{
    [Route("api/user")]
    public class UserController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                var users = dbContext.Users.Include(item => item.ImageInfos).Select(user => new UserFE() { 
                    Id = user.Id,
                    GroupId = user.GroupId,
                    Username = user.Username,
                    Password = user.Password,
                    IsDoctor = user.IsDoctor,
                    PersonGroupId = user.PersonGroupId,
                    PersonId = user.PersonId
                }).ToList();
                if (users.Count != 0)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetByIdUser(string id)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Include(item => item.ImageInfos).Single(item => item.Id == id);
                    var user = new UserFE()
                    {
                        Id = entity.Id,
                        GroupId = entity.GroupId,
                        Username = entity.Username,
                        Password = entity.Password,
                        IsDoctor = entity.IsDoctor,
                        PersonGroupId = entity.PersonGroupId,
                        PersonId = entity.PersonId
                    };
                    return Ok(user);
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

        [Route("api/user/byusername/")]
        [HttpGet]
        public IHttpActionResult GetByUserUsername(string username)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Include(item => item.ImageInfos).Single(item => item.Username == username);
                    var user = new UserFE()
                    {
                        Id = entity.Id,
                        GroupId = entity.GroupId,
                        Username = entity.Username,
                        Password = entity.Password,
                        IsDoctor = entity.IsDoctor,
                        PersonGroupId = entity.PersonGroupId,
                        PersonId = entity.PersonId
                    };
                    return Ok(user);
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
        public IHttpActionResult GetUserByPersonId(string personId)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Include(item => item.ImageInfos).Single(item => item.PersonId == personId);
                    var user = new UserFE()
                    {
                        Id = entity.Id,
                        GroupId = entity.GroupId,
                        Username = entity.Username,
                        Password = entity.Password,
                        IsDoctor = entity.IsDoctor,
                        PersonGroupId = entity.PersonGroupId,
                        PersonId = entity.PersonId
                    };
                    return Ok(user);
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
        [Route("api/user/bygroupid/")]
        public IHttpActionResult GetAllUsersByGroup(string groupId)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                var users = dbContext.Users.Where(user => user.GroupId == groupId).Include(item => item.ImageInfos).Select(user => new UserFE()
                {
                    Id = user.Id,
                    GroupId = user.GroupId,
                    Username = user.Username,
                    Password = user.Password,
                    IsDoctor = user.IsDoctor,
                    PersonGroupId = user.PersonGroupId,
                    PersonId = user.PersonId
                }).ToList();
                if (users.Count != 0)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet]
        [Route("api/user/login")]
        public IHttpActionResult LoginUser([FromBody] LoginRequest request)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Include(item => item.ImageInfos)
                        .Single(u => u.Username == request.Username);
                    if (Crypto.ComparePasswords(entity.Password, request.Password))
                    {
                        var user = new UserFE()
                        {
                            Id = entity.Id,
                            GroupId = entity.GroupId,
                            Username = entity.Username,
                            Password = entity.Password,
                            IsDoctor = entity.IsDoctor,
                            PersonGroupId = entity.PersonGroupId,
                            PersonId = entity.PersonId
                        };
                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
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

        [HttpPost]
        [Route("api/user/register")]
        public IHttpActionResult PostUser([FromBody] RegisterRequest request)
        {
            try
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    var isAlreadyRegistered = dbContext.Users.Any(entity => entity.Username == request.Username);
                    if (isAlreadyRegistered)
                    {
                        return BadRequest("User already exists");
                    }

                    var user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = request.Username,
                        Password = Crypto.EncryptPassword(request.Password),
                        IsDoctor = request.IsDoctor
                    };

                    if (!request.IsDoctor)
                    {
                        user.PersonGroupId = Guid.NewGuid().ToString();
                    }

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    return Ok(user);
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
        public IHttpActionResult PutUser([FromBody] User user)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Single(item => item.Id == user.Id);
                    entity.Username = user.Username;
                    entity.Password = Crypto.EncryptPassword(user.Password);
                    entity.GroupId = user.GroupId;
                    entity.PersonId = user.PersonId;
                    entity.PersonGroupId = user.PersonGroupId;
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
        public IHttpActionResult DeleteUser(string id)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var entity = dbContext.Users.Single(item => item.Id == id);
                    dbContext.Users.Remove(entity);
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
