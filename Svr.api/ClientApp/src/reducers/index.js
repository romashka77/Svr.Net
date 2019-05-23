import claimsLoadedType from '../action-types/claim-types';

const initialState = {
  claims: [
    {
      id: 1,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 2,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
    {
      id: 3,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 4,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
    {
      id: 5,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 6,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
    {
      id: 7,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 8,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
    {
      id: 9,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 10,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
    {
      id: 11,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 12,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
  ],
  columns: [
    {
      dataField: 'title',
      text: 'title',
      sort: true,
      //filter: textFilter()
    },
    {
      dataField: 'author',
      text: 'author',
      sort: true,
      //filter: textFilter()
    },
  ]
};

const reducer = (state = initialState, action) =>
{

  switch (action.type)
  {
    case claimsLoadedType:
      return {
        claims: action.payload.claims,
        columns: action.payload.columns
      };
    default:
      return state;
  }
};

export default reducer;
