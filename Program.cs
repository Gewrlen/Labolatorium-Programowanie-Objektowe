using System;
using System.Linq;
using SystemOcenianiaSimple.Repositories;
using SystemOcenianiaSimple.Models;
using SystemOcenianiaSimple.Utils;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var userRepo = new UserRepository();
var classRepo = new ClassRepository();
var assessmentRepo = new AssessmentRepository();
var gradebookRepo = new GradebookRepository();

User? currentUser = null;

while (true)
{
    Console.Clear();
    Console.WriteLine("=== System Oceniania (Simple) ===");
    Console.WriteLine("1. Zaloguj");
    Console.WriteLine("0. Wyjście");
    var c = Guard.ReadInt("Wybór: ");

    if (c == 0) return;

    if (c == 1)
    {
        var email = Guard.ReadNonEmpty("Email: ");
        var pass = Guard.ReadNonEmpty("Hasło (demo: 1234): ");
        currentUser = userRepo.Login(email, pass);

        if (currentUser == null)
        {
            Console.WriteLine("Błędny login lub hasło.");
            Console.ReadLine();
            continue;
        }

        break;
    }
}

while (true)
{
    Console.Clear();
    Console.WriteLine($"Zalogowano: {currentUser!.Email} | Rola: {currentUser.Role}");
    Console.WriteLine();

    Console.WriteLine("1. Lista klas");
    Console.WriteLine("2. Dodaj klasę (CRUD)");
    Console.WriteLine("3. Dodaj wpis oceniany (Assessment)");
    Console.WriteLine("4. Dziennik ocen (podgląd)");
    Console.WriteLine("5. Wystaw/Zmień ocenę (Upsert)");
    Console.WriteLine("0. Wyjście");

    var op = Guard.ReadInt("Wybór: ");

    if (op == 0) return;

    if (op == 1)
    {
        var classes = classRepo.GetAll();
        Console.WriteLine("\nKlasy:");
        foreach (var cl in classes)
            Console.WriteLine($"{cl.Id} | {cl.GroupName} | {cl.TermName}");
        Console.ReadLine();
    }
    else if (op == 2)
    {
        if (currentUser.Role != "Teacher")
        {
            Console.WriteLine("Brak uprawnień (tylko Teacher).");
            Console.ReadLine();
            continue;
        }

        Console.WriteLine("\n=== Dodaj klasę ===");
        var group = Guard.ReadNonEmpty("Nazwa grupy (np. LAB-2): ");
        var term = Guard.ReadNonEmpty("Semestr (np. 2025/2026 Zima): ");

        var id = classRepo.Add(new ClassGroup { GroupName = group, TermName = term });
        Console.WriteLine($"Dodano klasę. ID = {id}");
        Console.ReadLine();
    }
    else if (op == 3)
    {
        if (currentUser.Role != "Teacher")
        {
            Console.WriteLine("Brak uprawnień (tylko Teacher).");
            Console.ReadLine();
            continue;
        }

        Console.WriteLine("\n=== Dodaj wpis oceniany ===");

        var classes = classRepo.GetAll();
        Console.WriteLine("\nDostępne klasy:");
        foreach (var cl in classes)
            Console.WriteLine($"{cl.Id} | {cl.GroupName} | {cl.TermName}");

        var classId = Guard.ReadInt("Podaj ID klasy: ");

        var title = Guard.ReadNonEmpty("Podaj nazwę wpisu (np. Kolokwium 1): ");

        Console.WriteLine("\nWybierz kategorię:");
        Console.WriteLine("1 - Kolokwium");
        Console.WriteLine("2 - Projekt");
        Console.WriteLine("3 - Aktywność");
        var cat = Guard.ReadInt("Wybór: ");

        var category = cat switch
        {
            1 => "Kolokwium",
            2 => "Projekt",
            3 => "Aktywność",
            _ => "Kolokwium"
        };

        Console.WriteLine("\nTryb oceniania:");
        Console.WriteLine("1 - Grade (ocena)");
        Console.WriteLine("2 - Points (punkty)");
        var modeChoice = Guard.ReadInt("Wybór: ");

        string mode = (modeChoice == 1) ? "Grade" : "Points";
        decimal? maxPoints = null;

        if (mode == "Points")
            maxPoints = Guard.ReadDecimal("Podaj maks. punkty (np. 100): ");

        var weight = Guard.ReadDecimal("Podaj wagę (np. 1): ");

        try
        {
            var newId = assessmentRepo.Add(new Assessment
            {
                ClassId = classId,
                CreatedByUserId = currentUser.Id,
                Title = title,
                Category = category,
                GradingMode = mode,
                MaxPoints = maxPoints,
                Weight = weight
            });

            Console.WriteLine($"Dodano wpis oceniany. ID = {newId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Błąd dodawania assessmentu:");
            Console.WriteLine(ex.Message);
        }

        Console.ReadLine();
    }
    else if (op == 4)
    {
        Console.WriteLine("\n=== Dziennik ocen (podgląd) ===");

        var classes = classRepo.GetAll();
        Console.WriteLine("\nDostępne klasy:");
        foreach (var cl in classes)
            Console.WriteLine($"{cl.Id} | {cl.GroupName} | {cl.TermName}");

        var classId = Guard.ReadInt("Podaj ID klasy: ");

        var rows = gradebookRepo.GetByClassId(classId);

        Console.WriteLine();
        foreach (var r in rows)
        {
            var val = r.Mode == "Points"
                ? (r.Points.HasValue ? $"{r.Points} pkt" : "-")
                : (r.GradeValue.HasValue ? $"{r.GradeValue}" : "-");

            var abs = r.IsAbsent ? " (NB)" : "";
            Console.WriteLine($"{r.StudentName} [{r.IndexNo}] | {r.AssessmentTitle} | {val}{abs}");
        }

        Console.ReadLine();
    }
    else if (op == 5)
    {
        if (currentUser.Role != "Teacher")
        {
            Console.WriteLine("Brak uprawnień (tylko Teacher).");
            Console.ReadLine();
            continue;
        }

        Console.WriteLine("\n=== Wystaw/Zmień ocenę ===");

        var classes = classRepo.GetAll();
        Console.WriteLine("\nDostępne klasy:");
        foreach (var cl in classes)
            Console.WriteLine($"{cl.Id} | {cl.GroupName} | {cl.TermName}");
        var classId = Guard.ReadInt("Podaj ID klasy: ");

        var assessments = assessmentRepo.GetByClassId(classId);
        if (assessments.Count == 0)
        {
            Console.WriteLine("Brak wpisów ocenianych w tej klasie (dodaj w opcji 3).");
            Console.ReadLine();
            continue;
        }

        Console.WriteLine("\nWpisy oceniane:");
        foreach (var a in assessments)
            Console.WriteLine($"{a.Id} | {a.Title} | {a.Category} | {a.GradingMode}");

        var assessmentId = Guard.ReadInt("Podaj ID wpisu ocenianego: ");
        var chosen = assessments.FirstOrDefault(x => x.Id == assessmentId);
        if (chosen == null)
        {
            Console.WriteLine("Nie ma takiego wpisu.");
            Console.ReadLine();
            continue;
        }

        var rows = gradebookRepo.GetByClassId(classId)
            .Where(x => x.AssessmentId == assessmentId)
            .ToList();

        Console.WriteLine("\nStudenci:");
        foreach (var s in rows.Select(x => new { x.StudentId, x.StudentName, x.IndexNo }).Distinct())
            Console.WriteLine($"{s.StudentId} | {s.StudentName} | {s.IndexNo}");

        var studentId = Guard.ReadInt("Podaj StudentId: ");

        Console.WriteLine("Czy nieobecny? (t/n)");
        var abs = (Console.ReadLine() ?? "").Trim().ToLower() == "t";

        decimal? gradeValue = null;
        decimal? points = null;

        if (!abs)
        {
            if (chosen.GradingMode == "Grade")
            {
                gradeValue = Guard.ReadDecimal("Podaj ocenę (np. 4.5): ");
            }
            else
            {
                points = Guard.ReadDecimal("Podaj punkty: ");
            }
        }

        try
        {
            var enrollmentId = gradebookRepo.GetEnrollmentId(classId, studentId);
            gradebookRepo.UpsertGrade(enrollmentId, assessmentId, gradeValue, points, abs);

            Console.WriteLine("Zapisano.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Błąd zapisu:");
            Console.WriteLine(ex.Message);
        }

        Console.ReadLine();
    }
}
