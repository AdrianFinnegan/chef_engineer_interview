
# Chef Engineering Test - SDE

Thank you for applying for an Engineering role at Chef.  The next step
in your interview process is the completion of this assignment.  A
time will be - or already has been - scheduled to review the project
with a Chef engineering staff member.

During that review, you will be asked to walk the interviewer through your project
code and discuss your decisions.

If something is unclear or you need additional information, please ask
the contact you were provided when you received the link to this
assignment.

The maximum time we suggest spending on this assignment is four hours.


# Requirements:

Write a program that takes a pair of directories as input and writes
out three files as output.
* A file named 'common', which contains files that are identical in both the
  first and second directories.
* Files 'a\_only' and 'b\_only', that contains the files that are only in
  the first directory ('a') and those that are only in the second
  directory ('b')

A file is considered identical if its contents are byte for byte identical (name,
permissions and location don't matter.)

Expect that this will be run on very large directory trees (100,000
files) 


## General

* Use the language and (as necessary) frameworks of your choice.
* Ensure the following tasks exist and are automated via Makefile or other tool of your choice:
  * build
  * clean
  * test
  * run
* Your project should include testing at the level you find
  appropriate. Be prepared to discuss your testing choices.

* Modify this README.md to include any setup/instructions and any additional
  notes that you wish to communicate. The existing content can be replaced if you wish.
  * Ensure any build and runtime dependencies are captured in this README.md.

## Tips

* We are looking for a functional, maintainable, and readable solution
  over a perfectly written and optimized solution.

* The md5sum utility might be useful.

* There are not any 'tricks' intended in this problem. However, we
  (the authors), are human and can make mistakes. Please ask if
  anything is unclear.
  
## Installation Prerequisite 
The application relies on .Net Framework 3.5+. This can be downloaded from: https://www.microsoft.com/en-gb/download/details.aspx?id=21
  
## Compiling and Setup
The application supports the following commands, executing from a Windows Shell.

Navigate to the Project Root:
* > build - Builds the application.
* > build /t:Test - Runs a series of tests against the built application.
* > build /t:Build /t:Test - Builds the application and immediately runs the tests after a successful compilation.
* > build /t:Clean - Returns the application to its original unbuilt state; Removes any assemblies generated as part of the build

## Executing the application
Once successfully built; a directory comparison can be made using the following examples:
* > DirectoryAnalyser.exe C:\Source C:\Destination
* > DirectoryAnalyser.exe C:\Source .
* > DirectoryAnalyser.exe . C:\Source