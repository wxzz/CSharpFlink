# CSharpFlink
 a real-time computing framework
 
 官方网址 [http://www.ineuos.net/](http://www.ineuos.net/)
 
 技术博客 [https://www.cnblogs.com/lsjwq/](https://www.cnblogs.com/lsjwq/)
 
 作者QQ：504547114
 
 技术QQ群：54256083
 
### 1	项目背景
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;我们有一个全国性质的面向工业的公有云平台，通过专线或4G的链路方式实时向平台传输数据，每天处理1亿条左右的数据量，为现场用户提供实时的在线服务和离线数据分析服务。现在已经上线稳定运行有将近3年的时间。同时也为工业企业提供私有云建设服务。  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;我们计划使用Flink作为云平台后台的实时计算部分，基本实现数据点的聚合计算、表达式规则计算等业务，进一步实现机器学习或自定义复杂算法的需求。  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;我们经过将近一年左右时间的研究及开发，已经基本实现了聚合和逻辑等业务，但是感觉Flink比较重，并且应用和运维的水平要求比较高。  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;基于上述情况，我们自主使用[NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)开发一套[CSharpFlink](https://github.com/wxzz/CSharpFlink)实时计算组件，支持自定义数据源、计算和存储的基本要求。  

### 2	应用场景
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;主要面向物联网、工业互联网私有云或公有云平台建设过程中的数据点实时聚合和表达式计算。应用场景包括：
* 数据点的实时时间窗口范围内聚合计算，例如：最大值、最小值、平均值、和值、众数、方差、中位数等，可以自定义二次开发。
* 数据点的历史延迟窗口的一段时间范围内数据补充或更新的重新计算。
* 数据点的表达式计算，支持自定义C#脚本进行编辑，实时预警或数据深度加工处理。
* 主从结构的分布式部署，主节点负责计算任务分发，工作节点负责任务计算及结果存储。

### 3	框架特点
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;主要特点主要是根据我们多年的物联网、工业项目经验的提炼和总结，满足实现应用场景，特点包括：
* 使用最新的NET 5.0进行开发，完全跨平台。
* 实时数据窗口范围外的数据补发或更新的重新计算，例如：当前5秒的实时数据窗口，支持5秒以前的数据补充和更新，并且进行重新计算及更新到数据存储单元。
* 实时数据表达式计算支持定时计算或数据值改事件变触发计算，满足实时表达式或周期性计算。
* C#语言的二次开发，对接多种数据源，自定义算子和多种方式数据存储等。
* 单节点或分布式部署。

### 4	框架结构
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;框架结构组件的基本示意，如下图：
![CSharpFlink框架图示意图](https://img2020.cnblogs.com/blog/279374/202011/279374-20201117230431033-1640411941.png)

### 5	代码目录说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;使用VS2019进行工程开发，工程解决方案文件为：CSharpFlink.sln，代码目录说明如下：
* Cache：主节点和工作节点计算任务本地缓存管理。
* Calculate：计算任务输入、过程、输出操作及管理。
* Channel：主节点和工作节点分布式部署模式的IO通讯操作。
* Common：操作公众类库。
* Config：全局配置文件操作。
* Execution：全局工程的执行环境入口。
* Expression：表达式计算任务操作。
* Log：日志操作及管理。
* Model：数据点元数据信息。
* Node：主节点和工作节点管理。
* Protocol：主节点和工作节点之间分布式部署之间交互的协议。
* Sink：计算任务计算结果存储接口。
* Source：对接多种数据源接口，例如：mqtt、kafka、rabbitmq、数据库等。
* Task：窗口或表达任务接口，主节点和工作节点任务操作及管理。
* Window：数据窗口任务操作。
* Worker：工作节点接口。

### 6	配置文件说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;配置文件默认为：cfg\global.cfg，可以自定义指定配置文件，参见：命令行操作说明。配置文件说明，如下：
* MaxDegreeOfParallelism：任务并行度，主节点生成任务、工作节点处理任务依赖这个参数。
* MasterListenPort：主节点侦听端口，用于工作节点主动连接。
* MasterIp：主节点IP，用于工作节点主动连接。
* NodeType：节点运行模式，包括：Master、Slave和Both。
* RemoteInvokeInterval：远程调用工作节点间隔时间，单位：毫秒。
* RepeatRemoteInvokeInterval：调用工作节点失败后，重新调用工作节点间隔时间，单位：毫秒。
* SlaveExcuteCalculateInterval：工作节点执行计算任务间隔时间，单位：毫秒。
* MaxFrameLength：主节点和工作节点之间传输数据最大数据侦，单位：字节。
* WorkerPower：工作节点能力系数，大于1，会连续发送多个任务。

### 7	任务部署说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;二次开发参见：二次开发说明。开发好的任务，测试通过后，把程序集（.dll）复制到“tasks”目录下，例如工程TestTask项目测试、编译通过后，可以部署到“tasks”目录下，运行“CSharpFlink”主程序会自动加载和调用。
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;可以自定义指定任务程序集，参见：命令行操作说明。

### 8	命令行操作说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;命令行运行“CSharpFlink”程序，支持自定义指定配置文件或任务程序集，说明如下：
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-h         显示命令行帮助。
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-c         加载指定配置文件。 例如:CSharpFlink -c c:/my.cfg
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-t         加载任务程序集。     例如:CSharpFlink -t c:/mytask.dll
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;例如：
```c#
dotnet CSharpFlink.dll -c c:/master.cfg -t c:/mytask.dll
```

### 9	部署说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;“release”目录下是编译好的程序，把“CSharpFlink v1.0”分别复制到不同的路径下，分别修改“cfg\global.cfg”配置文件中“NodeType”参数为：Master和Slave，修改主节点程序“tasks\tasks.cfg”文件中的任务数，分别运行不同目录下的“dotnet CSharpFlink.dll”。
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;“TestTask.dll”源代码，参见：二次开发说明。

### 10	二次开发说明
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;二次开发主要针对数据源、计算过程和数据计算结果存储，大致过程如下：
* 数据源对接，可以自定义对接mqtt、kafka、rabbitmq、数据库等，需要继承SourceFunction接口，参见：RandomSourceFunction.cs类。
* 数据计算过程，可以自定义数据处理或加工，需要继承Calculate.Calculate接口，参见：聚合计算Avg.cs、表达式计算ExpressionCalculate.cs。通过AddWindowTask或AddExpressionTask函数参数进行实例化。
* 数据计算结果存储，可以自定义存储任何介质上，需要继承SinkFunction接口，参见：SinkFunction.cs类。

### 11	应用事例展示
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;同一台电脑，CPU：4核 I5-7400 3.0GHz,内存：16G，1个主节点，5个工作节点，生成1000个数据点任务，随机数据点时间窗口和计算算子，CPU使用率为：20%-30%，内存使用率：30%-40%，主节点CPU和内存使用情况：3%-5%、100MB-300MB, 工作节点CPU和内存使用情况：0.1%-2%、25MB-60MB。运行效果，如下图：
![CSharpFlink事例展示](https://img2020.cnblogs.com/blog/279374/202011/279374-20201117230605084-1962198174.png)

iNeuOS工业互联网公众号：

![iNeuOS工业互联网公众号](https://img2020.cnblogs.com/blog/279374/202011/279374-20201109210223158-1810580141.jpg)
