using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts;

public record RegistrationRequest(
    [Required]string Email, 
    [Required]string Username, 
    [Required]string Password);