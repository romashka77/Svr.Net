import React from 'react'; //1
import { render } from 'react-dom'; //2
import App from './app.jsx'; //3

render(
    <App />,
    document.getElementById('test')
); //4