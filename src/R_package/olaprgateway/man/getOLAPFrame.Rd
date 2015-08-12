\name{getOLAPFrame}
\alias{geOLAPFrame}
\title{Calls OLAPRGateway which calls the configured SQL Server SSAS instance and executes the specified MDX command to}
\usage{getOLAPFrame(command)}
\arguments{
  \item{command}{The MDX query to pass}
}
\value{
The resulting data.frame
}
\description{
Calls OLAPRGateway which calls the configured SQL Server SSAS instance and executes the specified MDX command to return a data.frame. NOTE: OLAPRGateway.exe must be running to use this method. OLAPRGateway can be obtained here: https://github.com/jdeats76/OLAPRGateway 
}
\examples{
getOLAPRFrame("select { [Measures].[Sales Amount] } ON 0, { [Date].[Fiscal].[Fiscal Year].&[2002] } ON 1 FROM [Adventure Works]")
}
