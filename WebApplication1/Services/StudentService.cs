﻿using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> studentCollection;

        public StudentService(IOptions<StudentDatabaseSettings> studentDbSettings) 
        {
            var mongoClient = new MongoClient(
                studentDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                studentDbSettings.Value.DatabaseName);

            studentCollection = mongoDatabase.GetCollection<Student>(
                studentDbSettings.Value.StudentsCollectionName);
        }

        public async Task<List<Student>> GetAsync() =>
            await studentCollection.Find(_ => true).ToListAsync();

        public async Task<Student> GetAsync(string id) =>
            await studentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task CreateAsync (Student newStudent) =>
            await studentCollection.InsertOneAsync(newStudent);

        public async Task UpdateAsync(string id,Student updatedStudent) =>
            await studentCollection.ReplaceOneAsync(x => x.Id == id, updatedStudent);

        public async Task RemoveAsync(string id) =>
           await studentCollection.DeleteOneAsync(x => x.Id == id);

        public async Task AddMultipleAsync(List<Student> students) =>
            await studentCollection.InsertManyAsync(students);

        public async Task<List<Student>> FilterByLastNameAsync(string lastName) =>
            await studentCollection.Find(student => student.LastName == lastName).ToListAsync();

        public async Task<List<Student>> GetYoungerThanAsync(int age) =>
            await studentCollection.Find(student => student.Age < age).ToListAsync();

        public async Task<List<Student>> GetOlderThanAsync(int age) =>
              await studentCollection.Find(student => student.Age > age).ToListAsync();
    }

}
