\name{setOLAPGatewayHost}
\alias{setOLAPGatewayHost}
\title{sets the OLAPRGatewayHost to be used by the other methods in the OLAPRGateway package}
\usage{setOLAPGatewayPort(port)}
\arguments{
  \item{port}{The hoat to be used by OLAPRGateway methods. Set to loop-back IP 127.0.0.1 if running OLAPRGateway on the system system R development is occuring}
}
\value{
This method returns nothing
}
\description{
sets the OLAPRGatewayHost to be used by the other methods in the OLAPRGateway package
NOTE: OLAPRGateway.exe must be running to use this method. OLAPRGateway can be obtained here: https://github.com/jdeats76/OLAPRGateway 
}
\examples{
setOLAPRGatewayHost("127.0.0.1")
}
 

