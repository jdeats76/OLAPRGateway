\name{getSQLFrame}
\alias{geSQLFrame}
\title{Calls OLAPRGateway which calls the configured SQL Server instance and executes the specified SQL command to return a data.frame.}
\usage{getSQLFrame(command)}
\arguments{
  \item{command}{The SQL query to pass}
}
\value{
The resulting data.frame
}
\description{
Calls OLAPRGateway which calls the configured SQL Server instance and executes the specified SQL command to return a data.frame. NOTE: OLAPRGateway.exe must be running to use this method. OLAPRGateway can be obtained here: https://github.com/jdeats76/OLAPRGateway 
}
\examples{
getOLAPRFrame("SELECT * FROM USER")
}
 
