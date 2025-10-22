Contract Monthly Claim System (ST10434391)

This application is designed to manage monthly claims for contract lecturers. It provides a simple, end-to-end workflow for lecturers to submit claims, upload supporting documents, and for coordinators or managers to review, approve, or reject these claims.

Core Features

This application implements the full lifecycle of a claim:

Lecturer Dashboard: Lecturers can view all their past and present claims in one place.

Claim Management:

Create new monthly claims.

Add individual work items (with hours, rate, and description) to a claim.

View running totals for hours and amount (in Rand R).

Document Upload: Securely upload supporting documents (.pdf, .docx, .xlsx) for each claim. The system validates file type and size (5MB limit).

Submission Workflow: A prominent "Submit" button allows lecturers to send their completed claims for review. Once submitted, claims are locked from further editing.

Transparent Status Tracking: A color-coded status system (Draft, Submitted, Approved, Rejected) provides a clear, at-a-glance view of where each claim is in the process.

Coordinator Dashboard: A separate view for Programme Coordinators and Academic Managers that lists all pending claims requiring their attention.

Approval Workflow: Approvers can review the full details of any claim, including attached documents, and then "Approve" or "Reject" it with optional comments.

Graceful Error Handling: The system provides user-friendly error messages for common issues (like invalid file uploads) and a custom "Oops!" page for any unexpected server errors.

Technology Stack

How to Run

This project is a self-contained prototype and does not require an external database.

Clone or download the repository.

Open the project in Visual Studio 2022.

Press F5 or the "Start Debugging" button to build and run the application.

Your browser will open, and you can navigate to the home page, which will present the "My Claims" and "Coordinator Dashboard" options.

Project Structure

The project follows a standard ASP.NET Core MVC pattern:

/Models: Contains the C# classes that define the application's data (e.g., Claim.cs, Lecturer.cs, Approval.cs).

/Views: Contains all the UI (Razor .cshtml files) organized into folders for each controller.

/Controllers: Contains the C# classes that handle user requests and business logic (e.g., ClaimsController.cs, CoordinatorController.cs).

/Helpers: Contains utility classes, such as the ClaimStatusHelper.cs for styling statuses.

/wwwroot: Contains all static assets like CSS (site.css), JavaScript, and the uploads folder where documents are stored.

Program.cs: The main entry point for the application, where services and middleware are configured.


This application uses an in-memory ConcurrentDictionary as its database (in ClaimsController.cs). This means all data, including new claims and uploaded files, will be lost every time the application is stopped and restarted. It is intended for demonstration and testing purposes only.
