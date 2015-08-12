validateTokenSet <-
function() {
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
