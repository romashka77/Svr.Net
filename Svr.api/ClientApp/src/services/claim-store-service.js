
export default class ClaimStoreService
{
  getClaims()
  {
    return [
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
    ];
  }

  getColumns()
  {
    return [
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
    ];
  }
}