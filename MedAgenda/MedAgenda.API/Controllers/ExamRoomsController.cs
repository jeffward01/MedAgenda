using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Models;
using AutoMapper;

namespace MedAgenda.API.Controllers
{
    public class ExamRoomsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/ExamRooms || Controller Method [0]
        public IEnumerable<ExamRoomModel> GetExamRooms()
        {
            return Mapper.Map<IEnumerable<ExamRoomModel>>(db.ExamRooms);
        }

        // GET: api/ExamRooms/5 || Get By ID [1]
        [ResponseType(typeof(ExamRoomModel))]
        public IHttpActionResult GetExamRoom(int id)
        {
            ExamRoom dbExamRoom = db.ExamRooms.Find(id);
            if (dbExamRoom == null)
            {
                return NotFound();
            }
            ExamRoomModel examRoom = Mapper.Map<ExamRoomModel>(dbExamRoom);

            return Ok(examRoom);
        }

        // PUT: api/ExamRooms/5 || Update Room [2]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutExamRoom(int id, ExamRoomModel examRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != examRoom.ExamRoomID)
            {
                return BadRequest();
            }

            var dbExamRoom = db.ExamRooms.Find(id);

            dbExamRoom.Update(examRoom);

            db.Entry(dbExamRoom).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamRoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ExamRooms || New Exam Room [3]
        [ResponseType(typeof(ExamRoomModel))]
        public IHttpActionResult PostExamRoom(ExamRoomModel examRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbExamRoom = new ExamRoom();

            dbExamRoom.Update(examRoom);            
            db.ExamRooms.Add(dbExamRoom);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Unable to add Exam Room to database");
            }

            examRoom.ExamRoomID = dbExamRoom.ExamRoomID;

            return CreatedAtRoute("DefaultApi", new { id = examRoom.ExamRoomID }, examRoom);
        }

        // DELETE: api/ExamRooms/5 || Delete Exam Room [4]
        [ResponseType(typeof(ExamRoomModel))]
        public IHttpActionResult DeleteExamRoom(int id)
        {
            ExamRoom examRoom = db.ExamRooms.Find(id);
            if (examRoom == null)
            {
                return NotFound();
            }

            db.ExamRooms.Remove(examRoom);

            try
            {               
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Could not delete Exam Room from database");
            }

            return Ok(Mapper.Map<ExamRoomModel>(examRoom));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExamRoomExists(int id)
        {
            return db.ExamRooms.Count(e => e.ExamRoomID == id) > 0;
        }
    }
}