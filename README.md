# SQLCipher Mini-Hack

### Challenge

In this challenge you will work with an existing Xamarin Studio project and convert it to use SQLCipher for secure encrypted storage.

The starting point is a simple application called "ShareDatabase" that can create, read, and optionally email a SQLite  database  containing a message. While the initial implementation prompts for a password, it doesn't actually do anything with it. Therefore the application is insecure, and the contents of the database can easily be viewed.  If you have an email-capabile test device you can run the application and send a copy of the database to yourself, inspect it, or open it with a SQLite browser.

To complete the challenge you must make a few  changes to convert the application to use SQLCipher. Once the database is encrypted it will requires a correct password to open, and the contents will be unreadable. You can share the message database with others over email, or open the database file offline to examine it. 

### Prerequisites

**Step 1.** Download the Trial version of SQLCipher from the Component Store at [https://components.xamarin.com/view/sqlcipher-for-xamarin/](https://components.xamarin.com/view/sqlcipher-for-xamarin/)

**Step 2.** Unzip the trial package and add the Mono.Data.Sqlcipher assemblies from lib/ios and lib/android as references under the respective ShareDatabase iOS and Android projects.

### Walkthrough 1 - Using sqlite-net

**Step 1.** Download the SQLCipher-enhanced sqlite-net source code from the following URL

[https://raw.github.com/zeteticllc/sqlite-net/master/src/SQLite.cs](https://raw.github.com/sqlcipher/sqlite-net/master/src/SQLite.cs)

Copy the new SQLite.cs file into the ShareDatabaseAndroid folder, replace the standard SQLite.c file already there. This file is shared across the Android and iOS projects, so it only needs to be modified once.

**Step 2.** For both projects, open Project Options, navigate to the Compiler settings and append USE\_SQLCIPHER\_SQLITE to the _Define Symbols_ so it reads:
```
DEBUG;USE_SQLCIPHER_SQLITE
```

**Step 3.** Open the MessageDbSqlite.cs class and change the GetConnection() method to call the SQLiteConnection constructor that includes the SQLCipher password, i.e. from:
```
public SQLiteConnection GetConnection() 
{
  return new SQLiteConnection(FilePath);
}
```
to
```
public SQLiteConnection GetConnection() 
{
  return new SQLiteConnection(FilePath,Password);
}
```
*Note: MessageDbSqlite.cs is shared across the Android and iOS projects, so it only needs to be changed once.*

### Walkthrough 2 - Using ADO.NET with Mono.Data.Sqlite

**Step 1.** Open MessageDbAdo.cs 

**Step 2.** Change Mono.Data.Sqlite to Mono.Data.Sqlcipher, i.e.:
```
using Mono.Data.Sqlite;
```
to 
```
using Mono.Data.Sqlcipher
```

**Step 3.** Change the GetConnection method to call SetPassword() on the SqliteConnection before Open(). i.e.  from:
```
public SqliteConnection GetConnection() 
{
  var connection = new SqliteConnection(String.Format("Data Source={0}", FilePath));
  connection.Open();
  return connection;
}
```
to 
```
public SqliteConnection GetConnection() 
{
  var connection = new SqliteConnection(String.Format("Data Source={0}", FilePath));
  connection.SetPassword(Password);
  connection.Open();
  return connection;
}
```

*Note: MessageDbAdo.cs is shared across the Android and iOS projects, so it only needs to be changed once.*

### Verify Results

* Launch the Application
* Enter some content into the message box, and click "Save"
* Enter a password when prompted. 
* Quit the application
* Launch it again. Entering an incorrect password should result in an error. 
* Quit the application.
* Launch it again. Entering the correct password should load the message contents.


### Bonus

The "Send" button will only work when run on an actual email-capabile device, so load the application on your own phone or tablet and run it. Click the "Send" button to package up the encrypted database:

* Email it to yourself so you can inspect it later, or 
* Send it to a colleague and they can open it on their own device running the ShareDatabse application

