import React from 'react'
import ReactDOM from 'react-dom'


import PrimaryContainer from './components/PrimaryContainer.js'
window.React = React
const {render} = ReactDOM


render(
    <PrimaryContainer />,
    document.querySelector(".react-container")
)

module.hot.accept()
