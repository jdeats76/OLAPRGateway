getNetworkHost <-
function() {
   out <- "127.0.0.1"
   result <- tryCatch({
      out <- OLAPNetworkHost  
   },
   error=function(cond) { },
   warning=function(cond) { },
   finally={ })

   return(out)
}
