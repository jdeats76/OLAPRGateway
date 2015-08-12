######################################################################################
##
## R package counterpart to OLAPRGateway application.
## (C)2015 Jeremy Deats
## v1.0.0.0
##
## LICENSE
## This source code is licensed under the GNU Public License and comes with ABSOLUTELY
## NO WARRANTY. By using this software you agree to assume all liability for events
## that may occur as a result of its use. You may modify and redistribute this
## software according to the terms of the GNU General Public License. The terms and
## conditions of the GNU Public License can be found
## here http://www.gnu.org/licenses/gpl.html
##
## PURPOSE
## This methods in this package facilitate communication between R and Microsoft SQL
## Server,SQL Azure and SQL Server Analytics Services. These methods are dependent on
## the OLAPRGateway.exe Windows application which must be running concurrently with
## the R programming enviroment.
##
##
#####################################################################################

## Sets OLAPNetorkToken value
setOLAPRGatewayToken <- function(token) {
   assign("OLAPNetworkToken", token, envir = .GlobalEnv)
   
}

## Sets OLAPNetorkToken value
setOLAPRGatewayHost <- function(host) {
   assign("OLAPNetworkHost", host, envir = .GlobalEnv)
   
} 

## Sets OLAPRGateway port number. Default is 8888
setOLAPRGatewayPort <- function(port) {
   assign("OLAPNetworkPort", port, envir = .GlobalEnv)
}


## Confirms OLAPNetworkToken has been set
validateTokenSet <- function() {
   out <- 'True'
   result <- tryCatch({
      tmp <- OLAPNetworkToken   
   },
   error=function(cond) {
       message("OLAPNetworkToken is not set. Use setOLAPRGateway() function to set token.")
       out <- 'False'
   },
   warning=function(cond) { return(NA) },
   finally={ })

   return(out)
}

## Default to 127.0.0.1 unless one has been defined to user
getNetworkHost <- function() {
   out <- "127.0.0.1"
   result <- tryCatch({
      out <- OLAPNetworkHost  
   },
   error=function(cond) { },
   warning=function(cond) { },
   finally={ })

   return(out)
} 

## Default to port 8888 unless one has been defined to user
getNetworkPort <- function() {
   out <- 8888
   result <- tryCatch({
      out <- OLAPNetworkPort   
   },
   error=function(cond) { },
   warning=function(cond) { },
   finally={ })

   return(out)
}


## function to communicate with OLAPRGateway which must be listening on assigned port
## command parameter should be Microsoft MDX compliant syntax
## Lib=MyDLL.DLL;Class=Namespace.Class;Param=Data to pass to method input string here
getDotNETFrame <- function(lib,class,command){
 
    out <- tryCatch({
    server_resp = "[]"
    while(server_resp=="[]") {
    con <- socketConnection(host=getNetworkHost(), port = getNetworkPort(), blocking=TRUE,
                            server=FALSE, open="w+")
 
    packet <- c(OLAPNetworkToken,'Lib=',lib,';Class=',class,';Param=',command);
    sendme <- paste(packet,collapse='')
  
    write_resp <- writeLines(sendme, con)
    server_resp <- readLines(con, 1)

    if (server_resp != "[]") {
    eval(parse(text=server_resp))
    }

    close(con)}
    },
    error=function(condition) {
        message(paste(c('Failed to connect with OLAPRGateway. Error message', condition)))
    },
    warning=function(condition) {
       message(paste(c('Warning:', condition)))
    },
    finally= { })

    return(sqltemp)
   
}



## function to communicate with OLAPRGateway which must be listening on assigned port
## command parameter should be Microsoft MDX compliant syntax
getOLAPFrame <- function(command){
 
    sqltemp <- NaN
    out <- tryCatch({
    server_resp = "[]"
    while(server_resp=="[]") {
    con <- socketConnection(host=getNetworkHost() , port = getNetworkPort(), blocking=TRUE,
                            server=FALSE, open="w+")
 
    sendme <- paste(c(OLAPNetworkToken,command), collapse='')
  
    write_resp <- writeLines(sendme, con)
    server_resp <- readLines(con, 1)

    if (server_resp != "[]") {
    eval(parse(text=server_resp))
    }

    close(con)}
    },
    error=function(condition) {
        message(paste(c('Failed to connect with OLAPRGateway. Error message', condition)))
    },
    warning=function(condition) {
       message(paste(c('Warning:', condition)))
    },
    finally= { })

    return(sqltemp)
   
}




## function to communicate with OLAPRGateway which must be listening on assigned port
## command parameter should be Microsoft T-SQL compliant syntax
getSQLFrame <- function(command){
  
    sqltemp <- NaN
    out <- tryCatch({
    server_resp = "[]"
    while(server_resp=="[]") {
    con <- socketConnection(host=getNetworkHost() , port = getNetworkPort(), blocking=TRUE,
                            server=FALSE, open="w+")
 
    sendme <- paste(c(OLAPNetworkToken,command), collapse='')
  
    write_resp <- writeLines(sendme, con)
    server_resp <- readLines(con, 1)

    if (server_resp != "[]") {
    eval(parse(text=server_resp))
    }
  
    close(con)}
    },
    error=function(condition) {
        message(paste(c('Failed to connect with OLAPRGateway. Error message', condition)))
    },
    warning=function(condition) {
       message(paste(c('Warning:', condition)))
    },
    finally= { })

    return(sqltemp)
   
}

###################################################################################################
##  Uncomment lines below to create R Package
##  For information on compiling and installation see https://cran.r-project.org/doc/contrib/Leisch-CreatingPackages.pdf 
###################################################################################################
##
#list <- c("setOLAPRGatewayToken", "setOLAPRGatewayPort", "setOLAPRGatewayHost", "validateTokenSet", "getNetworkPort", "getNetworkHost", "getDotNETFrame", "getSQLFrame", "getOLAPFrame")
#package.skeleton(name = "olaprgateway", list, environment = .GlobalEnv, path = ".", force = FALSE, code_files = character())

