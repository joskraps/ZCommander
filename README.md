![ZCommander](http://github.cerner.com/js022742/ZCommander/raw/master/Images/Skull-BrainIcon.png)


ZCommander
==========

Overview
--------
ZCommander allows your to create simulated load on your system or to load static/random data into an application. Each zombie has a task, and that task can be static or contain dynamic data. A zombie is configured to execute a task in seconds, and each zombie can be multiplied. This can be used to perform load or capacity planning.
Configuration information is stored in XML and is passed to the Zombie server when starting up.

This project is in it's very early stages and needs some fine tuning.

Configuration
---------

Configuration information is stored in a xml file and is pased to the zombie server when starting. The basic layout of the configuraton file is as follows:

```xml
<ZCommander>
  <ConnectionStrings Source="Data Source=INSTANCE;Initial Catalog=ZCommander;uid=USER;pwd=PASS" />
  <Logging Enabled="True" TracePath="C:\boom.log" ConnectionString="Data Source=INSTANCE;Initial Catalog=ZCommander;uid=USER;pwd=PASS" />
  <MacroPatterns DataFactorySymbol="{!" DataFactoryPattern="(\{!.*?!\})" AssemblySymbol="#!" AssemblyPattern="(\#!.*?!\#)"  StaticValueSymbol="^!" StaticValuePattern="(\^!.*?!\^)" TaskVariableSymbol="@!" TaskVariablePattern="(\@!.*?!\@)" />
  <Zombies>
...
  </Zombies>
  <Factories>
...
  </Factories>
  <Assemblies>
...
  </Assemblies>
</ZCommander>
```

* `<ConnectionStrings>` - contains the main connection string that will be the source for data factories. Different connection strings can be defind in SQL tasks if need be.
 

```xml
<ConnectionStrings Source="Data Source=INSTANCE;Initial Catalog=ZCommander;uid=USER;pwd=PASS" />
```

* `<Logging>` - sets up basic logging to save output to a trace file and/or log table in SQL
    - Enabled - whether or not any form of logging should take place.
    - TracePath - path to where the trace path should be saved. "" will disable tracing
    -ConnectionString -= the connection string to the ZCommander database if task results will be saved. 

```xml
<Logging Enabled="True" TracePath="C:\boom.log" ConnectionString="Data Source=INSTANCE;Initial Catalog=ZCommander;uid=USER;pwd=PASS" />
```

* `<MacroPatterns>` - This section allows you to customize which patterns are used for macro substitutions. Patterns are composed of two parts: Symbol and Pattern.
    - Symbol - this is used to indicate that the particular pattern exists in a string/statement.
    - Pattern - a pattern is a regular expression that is used to split the macro from the rest of the string.

```xml
<MacroPatterns DataFactorySymbol="{!" DataFactoryPattern="(\{!.*?!\})" AssemblySymbol="#!" AssemblyPattern="(\#!.*?!\#)"  StaticValueSymbol="^!" StaticValuePattern="(\^!.*?!\^)" />
```


Zombies
--------
A zombie is a container object that defines the frequency to which a task should be executed, which task should be executed, and how many instances of that zombie should be created.

* Example:

```xml
<Zombie Name="Test Zombie" Frequency="1" Multiplier="0">
      <TaskVariables>
        <Variable Type="Static" Reset="True">
          <Name>Boom_Guid</Name>
          <Value>^![NEWGUID]!^</Value>
        </Variable>
        <Variable Type="Static" Reset="True">
          <Name>Boom_Resident</Name>
          <Value>{![ResidentFactory].[0].[re_recnum].[RAND]!}</Value>
        </Variable>
      </TaskVariables>
      <Tasks>
        ...
      </Tasks>
</Zombie>
```

 * `<Zombie>`
     *  Name - name that will be logged for this zombie.
     *  Frequencey - time, in seconds, that the zombie will execute. Subsequent zombie executions are not dependent on the previous zombie completing.
     *  Multiplier - number of zombies that will spawn.
 *  `<TaskVariables>` - list of `<Variable>` nodes that will be exposed to all tasks defined for a zombie. Individual tasks can modify these variables as they execute.
     *  `<Variable>` - string variable that is visible to all tasks within a zombie. When there are multiple instances of a single zombie, each instance will have it's own set of variables that are not exposed to other instances.
         *  Type - can be Static or SQL. Static values are either set in the `<Value>` node or set by a macro substitution from one of the data factories.
         *  Reset - whether the value should return to the original value after each execution of that instance of the zombie.
         *  `<Name>` - identifier for the variable. This value needs to be unique.
         *  `<Value>` - either a string value or a macro substituion statement that will be set on execution of the zombie.

Tasks
--------
A task implements the ITask interface and is what is executed when a zombie's frequency has elapsed. A zombie can have multiple tasks that are executed in order as defined by the sequence on each task.

`<Task Name="TaskName" Type="SQL|SQLTrace|WebRequest">...</Task>`

The following tasks are defined:
###_SQL task (Type = "SQL")_#
 
A SQL task is a list of SQL statements that will be executed. The SQL statement can contain a mixture of macros, or a list of replacement values that will be used to modify the statement.

**Basic SQL task**
```xml
<Task Name="Test Script1" Type="SQL">
    <SQLStatements>
        <Statement ConnectionString="{PARENT}" OutputType="None|Scalar">
            <Text>select {![ResidentJSON].[0].[Recnum].[RAND]!} from resident where re_recnum = '380</Text>
            <OutputList>
                <Output Type="TaskVariable" Target="BOOM_ST_RECNUM" />
            </OutputList>
        </Statement>
        ...
    </SQLStatements>
</Task>
```

**SQL task using replacement values**
```xml
<Zombie Name="Test Zombie" Frequency="1" Multiplier="5">
    <Task Name="Test Script1" Type="SQL">
        <SQLStatements>
            <Statement ConnectionString="{PARENT}" OutputType="None|Scalar">
                <Text>select {![ResidentJSON].[0].[Recnum].[RAND]!} from resident where re_recnum = '380</Text>
                <OutputList>
                    <Output Type="TaskVariable" Target="BOOM_ST_RECNUM" />
                </OutputList>
            </Statement>
        </SQLStatements>
        <Replacements>
          <Replacement>
            <MatchPattern>RegEX pattern</MatchPattern>
            <ReplaceMethod>Text|EditObjectFromBinary</ReplaceMethod>
            <ReplaceValue>VALUE</ReplaceValue>
          </Replacement>
        </Replacements>
    </Task>
</Zombie>
```

####Parameters#

- `<SQLStatements>` - contains a list of `<Statement>` elements. You can embed data factory, assmebly, or static values directly into this string ( see below for how to reference these values),or replacements can be used.
    - `<Statement>`
        - ConnectionString - either `{PARENT}` to use the source connection string, or a custom SQL connection string can be defined.
        - OutputType - defines whether the results of the statement execution should be stored in a variable. Valid options are None or Scalar. None will not attempt to save or capture the results of the statement. Scalar will store the first column and first row of the result set. If None is not used, `<Output>` nodes in the `<OutputList>` define how/where the value should be stored.
        - `<Text>` The actual statement that will be executed. This can contain a mixer of text and macros
        - `<OutputList>` (optional) - list of `<Output>` nodes that define where to store results of the statement execution.
            - `<Output>`
                - Type -  Valid options are: TaskVariable. TaskVariable will store the results into a local variable in the current zombie.
                - Target - Name of the local variable to store the value into.
- `<Replacements>`(optional) - a list of replacement objects to be used. Replacements use a regular expression pattern to find text and replace with the replace value. Each `<MatchPattern>` regular expression is ran against the `<SQLText>` text. Replace method defines how the new value is generated and replaced in the original string.

The following are valid `<ReplaceMethod>` values:

 - **Text** - value in `<ReplaceValue>` is replaced in `<SQLText>` based on `<MatchPattern>` regular expression. The `<ReplaceValue>` can contain either static text or can contain a macro object to substitute.
    ```xml
    <Replacement Global="True|False">
      <MatchPattern>(?&lt;=Image1(.*)=.*)(\w)+</MatchPattern>
      <ReplaceMethod>Text</ReplaceMethod>
      <ReplaceValue>{![ResidentFactory].[0].[re_recnum].[RAND]!}</ReplaceValue>
    </Replacement> 
    ```

 - **EditObjectFromBinary** - When this is used, the regular expression pattern defined in `<ReplaceMethod>` will grab the value from the `<SQLText>` that will be deserialized into the selected object ( these are explained in the assembly section) and then re-formatted back into a string to be insterted into the `<SQLText>` (in the example `[TOBINARY]` tells it to re-serialize the object into binary before inserting into `<SQLText>`).
  - The text that is returned from the regular expression must be deserialized into the correct object before processing can continue.
- Any text outside of the macro identifiers ({!|!},#!|!#, or ^!|!^) in the `<ReplaceValue>` will be inserted as is without any formatting.    2

    ```xml
    <Replacement Global="True|False">
      <MatchPattern>(?&lt;=Image1(.*)=.*)(\w)+</MatchPattern>
      <ReplaceMethod>EditObjectFromBinary</ReplaceMethod>
      <ReplaceValue>0x#![UploadedObservation].[TOBINARY]!#</ReplaceValue>
    </Replacement> 
    ```

###_SQLTrace task (Type = "SQLTrace")_#
This task uses a trace file created using SQL profiler to modify/execute statements. This allows your to simulate predefined scenarios or loads either as is or with modified/random data. SQLTrace tasks utitlize replacement values to know what to change in each statement that is contained in the trace file.

**Example SQLTrace task**
```xml
<Zombie Name="TestTrace" Multiplier="1" Frequency="1">
  <Task Name="Test Trace" Type="SQLTrace" FilePath="C:\Users\JS022742\Desktop\SQLTraceFile.trc">
    <EventsToMonitor>
      <Event Text="RPC:Starting" />
    </EventsToMonitor>
    <Replacements>
      <Replacement>
        <MatchPattern>(?&lt;=Image1(.*)=.*)(\w)+</MatchPattern>
        <ReplaceMethod>EditObjectFromBinary</ReplaceMethod>
        <ReplaceValue>0x#![UploadedObservation].[TOBINARY]!#</ReplaceValue>
      </Replacement>          
      <Replacement>
        <MatchPattern>5/13/2014 1:37:08 PM</MatchPattern>
        <ReplaceMethod>Text</ReplaceMethod>
        <ReplaceValue>^![DATETIME].[NOWUTC].[NONE]!^</ReplaceValue>
      </Replacement>
      <Replacement>
        <MatchPattern>02121bfa-be6f-4fb2-a4ed-3af90b8a8f62</MatchPattern>
        <ReplaceMethod>Text</ReplaceMethod>
        <ReplaceValue>^![NEWGUID]!^</ReplaceValue>
      </Replacement>
    </Replacements>
  </Task>     
</Zombie>   
```      
####Parameters#
 - FilePath - defines the path to the SQL trace file to be used.
 - `<EventsToMonitor>` - list of SQL events that will be used by this zombie from the trace file. Events are in the form of `<Event Text="RPC:Starting" />` where the Text attribute is the name of the event.  [Event list is located here:](http://technet.microsoft.com/en-us/library/ms175481.aspx)
 - `<Replacements>` - see explanation in SQL task

###_Web Service task (Type = "WEBSERVICE")_###

Macro substitutions
------------------

Data can be exposed to zombies and consumed utilizing macro substituions. There are three kinds of macro substitution: Data factories, assemblies, and static values.

###Data Factories###

Data factories load data into a dataset and expose that data to tasks. Data is accessed using the factory name, table index, column name, and access method to provide a single result back to the task.

The following macro format must be used to substitute factory data into a statement:   `{![FACTORYNAME].[TABLEINDEX].[COLUMNNAME].[ACCESS_METHOD]!}`
 * `[FACTORYNAME]`: Name of the factory to utilize.
 * `[TABLEINDEX]`: Zero based index of which table to use
 * `[COLUMNNAME]`: String or zero based index of which column to use
 * `[ACCESS_METHOD]`: Method to select a row within the dataset. Valid options are MIN, MAX, or RAND.

New factories can be created and must implement the IDataFactory interface. Common parameters that are shared among data factorys are:
- `<Name>` - Name of the factory. This will be the key to access in macros.
- `<Type>` - Type of factory
- `<Refresh>` - Boolean value indicated whether the factory should be refreshed automatically.
- `<RefreshRate>` - Rate, in seconds, that the factory should be refreshed.

Example:
```xml
<Factory Name="ResidentFactory" Type="SQL" Refresh="True" RefreshRate="30">
```

###Types of Data Factories#

**SQL Data Factory**
A SQL data factory uses a fill statment to load a dataset from a SQL database. SQL data factories are defined in the `<Factories>` node.

 * Syntax to access - `{![FactoryName}.[DefaultValue]!}`
```xml
<Factory Name="ResidentFactory" Type="SQL" Refresh="True" RefreshRate="30">
  <ConnectionString>{PARENT}</ConnectionString>
  <FillQuery>select re_recnum from dbo.resident</FillQuery>
</Factory>
```  

#####Parameters#
 - `<ConnectionString>`: Connection string to use when filling/refreshing the factory. {PARENT} to use the source connection string, otherwise use defined connection string.
 - `<FillQuery>` - Query that will be used to fill or refresh the factory.

**JSON Data Factory** - uses a file path or web service URL to load JSON into a dataset. Because this uses anonymous types, the format of the JSON must be a list of identical objects.

Example:
```json
[
	{
	  "FirstName": "Joel",
	  "LastName": "Sparks",
	  "Recnum": 1
	}, {
	  "FirstName": "Bob",
	  "LastName": "Hope",
	  "Recnum": 2
	}, {
	  "FirstName": "Boom",
	  "LastName": "Shockalocka",
	  "Recnum": 3
	}, {
	  "FirstName": "Terd",
	  "LastName": "Ferguson",
	  "Recnum": 4
	}
]
```

The factory is defined with a type of JSON and has a load path property which defines the source of the JSON.

```xml
<Factory Name="ResidentJSON" Type="JSON" Refresh="False" RefreshRate="30">
  <LoadPath>C:\Git\ZCommander\master\ZCommander.TestConsole\SampleJSONFiles\ResidentJSON.json</LoadPath>
</Factory>
```
#####Parameters#
 - `<LoadPath>`: Either a file path to the file containing the JSON or a URL to the web service that will return the desired JSON

###Zombie local variables#
Local variables are specific to an instance of a zombie. These values can be static, or a value from a sql statement or macro substitution. Local variable are defined at the root of Zombie nodes.

 * Syntax to access - `@![VariableName}.[DefaultValue]!@`
     * VariableName- key/identifier for the variable
     * DefaultValue - value to use if key cannot be found.
    
```xml
  <TaskVariables>
    <Variable Type="Static" Reset="True">
      <Name>Boom_Guid</Name>
      <Value>^![NEWGUID]!^</Value>
    </Variable>
    <Variable Type="Static" Reset="True">
      <Name>Boom_Resident</Name>
      <Value>{![ResidentFactory].[0].[re_recnum].[RAND]!}</Value>
    </Variable>
  </TaskVariables>
```

###Assemblies#
Assemblies allow objects to be loaded via reflection and serialized for use in task statements. Properties in that object can be set with data factory, assembly, or static values.

The following macro format must be used to substitute factory data into a statement: `#![ASSEMBLYNAME].[SERIALIZATIONMETHOD]!#`
- `[ASSEMBLYNAME]`: Name of assembly to use
- `[SERIALIZATIONMETHOD]`: Type of serialization to use once properties have been set (if present). Currently, the only serialization method is TOBINARY

Example:
```xml
<Assemblies>
  <Assembly Name="AssemblyName" Object="FullObjectNameInDLL" Path="FilePathToDLL">
      <Properties>
        ...
      </Properties>
  </Assembly>
</Assemblies>
```
#####Parameters#
 - Name -  Name of the assmebly. This will be the key to access in macros.
 - Object - Full name of the object in the assembly (namespace included).
 - Path - Full path to the DLL.

You can edit the properties of an object through the `<Properties>` list.
```xml
<Properties>
  <Property>
    <Name>PublicPropertyNameInObject</Name>
    <Type>DataType</Type>
    <Value>NewValue</Value>
  </Property>
</Properties>
```

 - `<Name>` - the property name as it exists in the object
 - `<Type>` - data type of the property.
    - Valid types: Int32,datetime,datetimehlper(see datetimehelper usage below in static value),bool, and string
 - `<Value>` - the new value that will be used. This can be a static value or a macro value.

###Static Values#

 -  Syntax to access: `^![TYPE].[OPTION1].[OPTION2]!^`
 -  Valid Types:
    + DATETIME : `^![DATETIME].[NOW|NOWUTC].[STRINGFORMATTER]!^`
       - The third option can specify a Microsoft string format to be applied to the date.
       - Example: `^![DATETIME].[NOWUTC].[NONE]!^`,`^![DATETIME].[NOWUTC].[{0:MM/dd/yy}]!^`
    + DATETIMEHELPER : `'^![DATETIMEHELPER].[Relative date string].[STRINGFORMATTER]!^'`
       - `[Relative date string]` - this is a string to convert into a date. It is in the format of: [Integer (day|days|month|months|year|years) (ago|from now)]
          * Examples: `[2 days from now]` , `[2 days ago]`, `[2 months from now]`, `[2 months ago]`
       - Currently, a time of midnight will be stamped when this option is used.
       - The third option can specify a Microsoft string format to be applied to the date.
       - Examples:`^![DATETIMEHELPER].[2 days from now].[NONE]!^` , `^![DATETIMEHELPER].[2 days from now].[{0:MM/dd/yy}]!^`
    + Guid : `^![NEWGUID]!^`
    + Text : `^![STRING].[FILL].[CHARACTER].[COUNT]!^`
       * `[CHARACTER]` - the single character that will be used to fill a string
       * `[COUNT]` - either a static number, or a range of numbers to randomly fill the string with. If a range is used, it needs to be in the form min-max ( `[1-100]` )


Sample config file
================
```xml

```
