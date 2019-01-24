import posed from 'react-pose'
var pageID=100000

const Hideable = posed.div({
    visible : { opacity : 1},
    hidden : {opacity : 0}
});


/*
    Description:
        Wraps a React component in the Hideable pose animation such that
        if currentPage (app state) does not match the expected page,
        the content is invisible, otherwise it fades in.
    Params:
        ele - react component to wrap
        currentPage - this.state.page of PrimaryContainer
        expectedPage - the page name when wrapped component will be visible.
    Return:
        The wrapped component.


*/
const MakeHideable = (ele,currentPage,expectedPage)=>{
        return(
         <Hideable 
             className="page" 
             style=
             {
                currentPage==expectedPage ?
                 {zIndex:1000}
                 :
                 {zIndex:-1000,display:'none'}
                
            }  
            pose={currentPage==expectedPage ? 'visible' : 'hidden'}>
            {ele}
         </Hideable>
         )
    }


export {MakeHideable};