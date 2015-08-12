getOLAPFrame <-
function(command){
 
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
