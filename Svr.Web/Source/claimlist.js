import React from "react";
import ReactDOM from "react-dom";
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import { allReducers } from './Reducers';
import WebPage from './Components/WebPage';

const store = createStore(allReducers);


ReactDOM.render(
    <Provider store={store}>
        <WebPage />
    </Provider>,
    document.getElementById("fieldToShow")
);



//module.hot.accept();