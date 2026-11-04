
using System.Collections.Generic;

namespace MatMentor.Mobile.Models;

public record CreateStudentProfileRequest(
    string Username,
    string Password,
    string ParentEmail,
    int Grade,
    List<int> InterestIds
);
