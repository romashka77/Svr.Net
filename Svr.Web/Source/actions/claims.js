const mockApiData = [
  {
    id: 1,
    name: 'test1',
  },
  {
    id: 2,
    name: 'test2',
  },
  {
    id: 3,
    name: 'test4',
  },
];

const getClaims = () => (dispatch) => {
  setTimeout(() => {
    dispatch({ type: 'FETCH_CLAIMS_SUCCESS', payload: mockApiData });
  }, 2000);
};
export default getClaims;
// export const getClaims = () => {
//    return dispatch => {
//        setTimeout(() => {
//            console.log('Получить иски');
//            dispatch({ type: 'FETCH_CLAIMS_SUCCESS', payload: [] });
//        }, 2000);
//    };
// };
