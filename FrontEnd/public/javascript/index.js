import React from 'react'
import ReactDOM from 'react-dom'


import MainBox from './components/MainBox.js'
window.React = React
const {render} = ReactDOM
const MainElement = <MainBox/>

render(
    MainElement,
    document.querySelector(".react-container")
)

module.hot.accept()
