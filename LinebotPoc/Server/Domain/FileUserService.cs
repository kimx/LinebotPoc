﻿using LinebotPoc.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace LinebotPoc.Server.Domain
{
    public class FileUserService : IUserService
    {
        IWebHostEnvironment HostEnvironment;
        public FileUserService(IWebHostEnvironment hostEnvironment)
        {
            this.HostEnvironment = hostEnvironment;
            string dir = Path.Combine(HostEnvironment.WebRootPath, "Data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public Task Create(DummyUserDto userDto)
        {
            return this.Update(userDto);
        }

        public Task Update(DummyUserDto userDto)
        {
            string json = JsonSerializer.Serialize(userDto);
            string dir = Path.Combine(HostEnvironment.WebRootPath, "Data");
            string filePath = Path.Combine(dir, userDto.UserEmail + ".json");
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Write(json);
                }
            }
            return Task.CompletedTask;

        }

        public List<DummyUserDto> GetUsers()
        {
            string dataPath = Path.Combine(HostEnvironment.WebRootPath, "Data");
            List<DummyUserDto> dummyUserDtos = new List<DummyUserDto>();
            foreach (var filePath in Directory.GetFiles(dataPath, "*.json"))
            {
                string json = System.IO.File.ReadAllText(filePath);
                DummyUserDto dummyUserDto = JsonSerializer.Deserialize<DummyUserDto>(json);
                dummyUserDtos.Add(dummyUserDto);
            }
            return dummyUserDtos;
        }

        public DummyUserDto GetUser(string userEmail)
        {
            string filePath = Path.Combine(HostEnvironment.WebRootPath, "Data", userEmail + ".json");
            if (!File.Exists(filePath))
                return null;
            string json = System.IO.File.ReadAllText(filePath);
            DummyUserDto dummyUserDto = JsonSerializer.Deserialize<DummyUserDto>(json);
            return dummyUserDto;
        }

        public DummyUserDto GetUserByLine(string lineUserId)
        {
            string dataPath = Path.Combine(HostEnvironment.WebRootPath, "Data");
            foreach (var filePath in Directory.GetFiles(dataPath, "*.json"))
            {
                string json = System.IO.File.ReadAllText(filePath);
                DummyUserDto dummyUserDto = JsonSerializer.Deserialize<DummyUserDto>(json);
                if (dummyUserDto.LineUserId == lineUserId)
                    return dummyUserDto;
            }
            return null;
        }

        public DummyUserDto GetUserByLineNonce(string lineNonce)
        {
            string dataPath = Path.Combine(HostEnvironment.WebRootPath, "Data");
            foreach (var filePath in Directory.GetFiles(dataPath, "*.json"))
            {
                string json = System.IO.File.ReadAllText(filePath);
                DummyUserDto dummyUserDto = JsonSerializer.Deserialize<DummyUserDto>(json);
                if (dummyUserDto.LineNonce == lineNonce)
                    return dummyUserDto;
            }
            return null;
        }

        public Task Delete(DummyUserDto userDto)
        {
            string filePath = Path.Combine(HostEnvironment.WebRootPath, "Data", userDto.UserEmail + ".json");
            if (File.Exists(filePath))
                File.Delete(filePath);
            return Task.CompletedTask;
        }

        public Task InitAsync(string connectionString)
        {
            return Task.CompletedTask;
        }
    }
}
