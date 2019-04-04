const initialState = [];


export default (state = initialState, action) => {
  if (action.type === 'ADD_CLAIM') {
    return [
      ...state,
      action.payload,
    ];
  } if (action.type === 'FETCH_CLAIMS_SUCCESS') {
    return action.payload;
  }
  return state;
};
