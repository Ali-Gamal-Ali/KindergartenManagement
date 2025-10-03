using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Xml.Linq;
using WalaaKidsApp.DataAccess;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Business.Services
{
    public static class ClassroomServices
    {
        public static async Task<List<Classroom>> GetAllClassrooms()
        {
            using (var context = new AppDbContext())
            {
                return await context.Classrooms.Include(cr=>cr.Students)
                    .ToListAsync();
            }
        }
        public static async Task AddClassroom(Classroom classroom)
        {
            using (var context = new AppDbContext())
            {
                context.Classrooms.Add(classroom);
                await context.SaveChangesAsync();
            }
        }
        public static async Task<bool> CheckDuplicatedClassroom(string name)
        {
            using (var context = new AppDbContext())
            {
                return await context.Classrooms.AnyAsync(cr => string.Equals(cr.Name,name));
            }
        }

        public static async Task ChangeNameAsync(Classroom selectedClassroom,string newName)
        {
            using (var context = new AppDbContext())
            {
                var isDuplicated = await context.Classrooms.AnyAsync(cr => string.Equals(cr.Name, newName));
                if (isDuplicated)
                    throw new ArgumentException("يوجد فصل مسجل بنفس الاسم");
                var classroom = await context.Classrooms.FindAsync(selectedClassroom.Id);
                classroom.Name = newName;
                await context.SaveChangesAsync();
            }
        }

        internal static async Task RemoveClassroomAsync(Classroom selectedClassroom)
        {
            using (var context = new AppDbContext())
            {
                context.Entry(selectedClassroom).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }
    }
}
