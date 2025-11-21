Contract Monthly Claim System (ST10434391)
Github link: https://github.com/MavundlaNkosana/PROG6212_POE_ST10434391

User Manual & Documentation

Version: 3.0

Platform: ASP.NET Core MVC

1. System Overview

The Contract Monthly Claim System (CMCS) is a web-based platform designed to digitize and automate the submission, verification, and payment processing of monthly claims for contract academic staff.

This manual provides detailed instructions for all three user roles:

Lecturers: Submit claims and upload documents.

Coordinators/Managers: Verify and approve claims using automated audit tools.

HR/Finance: Manage staff data and generate payment invoices.

2. Getting Started (Installation)

This system is designed as a self-contained prototype requiring no external database installation.

Prerequisites: Ensure Visual Studio 2022 and .NET 6/7/8 SDK are installed.

Open Project: Double-click the .sln solution file.

Run: Press F5 or click the green "Start Debugging" button.

Access: The application will launch in your default web browser (usually at https://localhost:xxxx).

3. Role-Based User Guide

A. For Lecturers (Submission)

Goal: Accurately calculate hours and submit a claim for payment.

Access Dashboard: Click "Lecturer" on the home page.

Create Claim: Click "Create New Claim" and select the Month/Year.

Add Work Items (With Automation):

Enter the date and description.

Auto-Calculation: Type your hours in the "Hours Worked" box. The system will instantly calculate your total payout (Hours × Rate) using JavaScript.

Note: The system prevents entering invalid hours (e.g., > 24 hours/day).

Upload Evidence:

Scroll to "Supporting Documents".

Select a file (.pdf, .docx, .xlsx only, max 5MB) and click Upload.

Final Submission:

Review your claim summary.

Click the green "Submit Final Claim" button.

Warning: Once submitted, the claim is locked and cannot be edited.

B. For Coordinators (Verification)

Goal: Review pending claims and ensure compliance with policy.

Access Dashboard: Click "Approver" on the home page.

Review List: Select any claim marked as Submitted.

Automated System Audit (New Feature):

Look for the "Automated System Audit" box at the top of the review page.

Green: The claim is perfect (Valid hours, documents attached, correct rates).

Yellow: Warnings detected (e.g., missing documents).

Red: Critical errors (e.g., rate tampering). Approval is disabled.

Decision:

Enter optional comments.

Click Approve or Reject.

The status updates instantly on the Lecturer's dashboard.

C. For HR & Finance (Administration)

Goal: Manage staff data and process payments.

Access Dashboard: Click "HR Admin" on the home page.

Manage Lecturers:

Click "Go to Directory" to view all staff.

Click Edit to update a lecturer's email, staff number, or Hourly Rate.

Note: Changing the hourly rate here updates the logic for all future claims.

Generate Invoices:

Click "View Reports" to see all Approved claims ready for payment.

Click "Generate Invoice" next to a claim.

A professional, printable invoice will open in a new tab. Use Ctrl+P to print or save as PDF.

4. Technical Architecture

Technology Stack

Backend: C# ASP.NET Core MVC

Frontend: Razor Views, Bootstrap 5, jQuery (for real-time calculation)

Data Storage: In-Memory ConcurrentDictionary (Simulated Database)

Reporting: LINQ-generated HTML views

Key Components

ClaimsController: Handles creation, editing, and submission logic.

CoordinatorController: Manages approval workflows and integrates the Verification Service.

HRController: Handles administrative tasks and invoice generation.

ClaimVerificationService: A business logic engine that validates claims against rules (Max hours, Rates, etc.).

VerificationResult: Data model returned by the service containing Errors and Warnings.

5. Troubleshooting & Notes

Data Persistence: As this is a prototype, all data is lost when the application stops. The system re-seeds a demo lecturer ("Dr. Thabo") and a demo claim on every restart.

Browser Support: Optimized for Chrome, Edge, and Firefox.

File Uploads: Files are stored in the wwwroot/uploads folder temporarily.

Support: For technical issues, please contact the IT department or refer to the source code documentation in Program.cs.
