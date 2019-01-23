import posed from 'react-pose'
var pageID=100000

const Hideable = posed.div({
    visible : { opacity : 1},
    hidden : {opacity : 0,display:'hidden'}
});



const MakeHideable = (ele,currentPage,expectedPage)=>{
       
        return(
         <Hideable 
         className="page" 
         style={
            currentPage==expectedPage ?
             {zIndex:1000}
             :
             {zIndex:-1000}
            
        }  
            pose={currentPage==expectedPage ? 'visible' : 'hidden'}>
            {ele}
         </Hideable>
         )
    }


export {MakeHideable};