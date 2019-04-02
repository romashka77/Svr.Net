import { combineReducers } from 'redux';
import claims from './claims';
import regions from './regions';

export default  combineReducers({
    claims,
    regions
})
