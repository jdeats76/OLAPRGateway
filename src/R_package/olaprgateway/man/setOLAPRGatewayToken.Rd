\name{setOLAPGatewayToken}
\alias{setOLAPGatewayToken}
\title{sets the OLAPRGatewayTpken to be used by the other methods in the OLAPRGateway package}
\usage{setOLAPGatewayPort(token)}
\arguments{
  \item{token}{The security token to use when calling OLAPRGateway.exe. The default value on the server is 7ee88c36}
}
\value{
This method returns nothing
}
\description{
sets the OLAPRGatewayTOken to be used by the other methods in the OLAPRGateway package
NOTE: OLAPRGateway.exe must be running to use this method. OLAPRGateway can be obtained here: https://github.com/jdeats76/OLAPRGateway 
}
\examples{
setOLAPRGatewayToken("7ee88c36")
}
