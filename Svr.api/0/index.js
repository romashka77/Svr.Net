import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'react-router-redux';
import { createBrowserHistory } from 'history';
import configureStore from './store/configureStore';
import App from './App';
import registerServiceWorker from './registerServiceWorker';

import getClaims from './claims-data';

// Create browser history to use in the Redux store
//Создание истории браузера для использования в магазине Redux
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const history = createBrowserHistory({ basename: baseUrl });

console.log('baseUrl', baseUrl);
console.log('history', history);

// Get the application-wide store instance, prepopulating with state from the server where available.
//Получите экземпляр хранилища для всего приложения, предварительно заполненный состоянием с сервера, где он доступен.
const initialState = window.initialReduxState;
const store = configureStore(history, initialState);

const rootElement = document.getElementById('root');

ReactDOM.render(
  <Provider store={store}>
    <ConnectedRouter history={history}>
      <App />
    </ConnectedRouter>
  </Provider>,
  rootElement);

registerServiceWorker();
//getClaims();