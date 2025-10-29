import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AtraccionList } from './atraccion-list';

describe('AtraccionList', () => {
  let component: AtraccionList;
  let fixture: ComponentFixture<AtraccionList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AtraccionList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AtraccionList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
