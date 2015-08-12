\name{getDotNETFrame}
\alias{getDotNETFrame}
\title{Calls OLAPRGateway which locates the matching .NET plug-in and returns a data.frame.}
\usage{getDotNETFrame(lib, class, command)}
\arguments{
  \item{lib}{Target .NET class library that has been loaded in OLAPRGateway (e.g. MyGatewayplugin.dll) }
  \item{class}{Target .NET Namespace and Class to invoke. (e.g. MyGatewayplugin.SimplePlugin)}
  \item{command}{Parameters to pass to Target .NET Class. }
}
\value{
The resulting data.frame
}
\description{
Calls OLAPRGateway which locates the matching .NET plug-in and returns a data.frame.
NOTE: OLAPRGateway.exe must be running to use this method. OLAPRGateway can be obtained here: https://github.com/jdeats76/OLAPRGateway 
}
\examples{
getDotNETFrame("MyLib.DLL","MyLib.MyClass1","Data to pass")
}
