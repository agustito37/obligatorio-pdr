﻿namespace Shared;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public static string Encoder(Object user)
    {
        User usr = (User)user;
        return $"{usr.Id}|{usr.Username}|{usr.Password}";
    }

    public static User Decoder(string encoded)
    {
        string[] parts = encoded.Split("|");

        return new User()
        {
            Id = Int32.Parse(parts[0]),
            Username = parts[1],
            Password = parts[2],
        };
    }
}
