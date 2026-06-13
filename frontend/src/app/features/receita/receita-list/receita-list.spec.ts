import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceitaList } from './receita-list';

describe('ReceitaList', () => {
  let component: ReceitaList;
  let fixture: ComponentFixture<ReceitaList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceitaList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReceitaList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
