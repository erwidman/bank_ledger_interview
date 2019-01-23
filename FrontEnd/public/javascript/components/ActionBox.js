import posed from 'react-pose'

const Hideable = posed.div({
    visible : { opacity : 1},
    hidden : {opacity : 0,display:'hidden'}
});

export default {Hideable};