getNetworkPort <-
function() {
   out <- 8888
   result <- tryCatch({
      out <- OLAPNetworkPort   
   },
   error=function(cond) { },
   warning=function(cond) { },
   finally={ })

   return(out)
}
